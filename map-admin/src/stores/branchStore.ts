import { defineStore } from 'pinia'
import axios from 'axios'

// Полная структура данных из JSON
export interface Break {
    BreakFrom: string;
    BreakTo: string;
}

export interface WorkDay {
    WorkingDay: string;
    DayOfWeek: string; //For separate deps
    WorkFrom: string;
    WorkTo: string;
    Breaks: Break[];
}

export interface TimeTable {
    Workstation: string;
    WorkDays: WorkDay[];
}

export interface CashDepartment{
    CashDescription: string;
    WorkDays: WorkDay[];
}

export interface BranchDetails {
    DepartmentId: number;
    DepartmentType: 'department' | 'atm' | 'terminal';
    DepartmentName: string;
    IsTemporaryClosed: boolean;
    FullAddress: string;
    ExtraServices: string[];
    Address: {
        BaseCity: string;
        City: string;
        DetailedAddress?: string;
        GeoLocation: {
            Lat: number;
            Long: number;
        };
    };
    TimeTables: TimeTable[];
    CashDepartments: CashDepartment[]; 
}

export interface BankMarker {
    id: number;
    departmentType: string;
    name: string;
    baseCity: string;
    fullAddress: string;
    latitude: number;
    longitude: number;
    isTemporaryClosed: boolean;
    dataJson: string;
    details?: BranchDetails;
}

export const useBranchStore = defineStore('branchStore', {
    state: () => ({
        rawBranches: [] as BankMarker[],
        isLoading: false,
        selectedBranch: null as BankMarker | null,
        filterType: 'all', 
        filterCity: 'all'
    }),

    getters: {
        uniqueCities: (state) => {
            const cities = state.rawBranches.map(b => b.baseCity);
            return ['all', ...new Set(cities)].sort();
        },

        filteredBranches: (state) => {
            return state.rawBranches.filter(b => {
                const matchesType = state.filterType === 'all' || b.departmentType === state.filterType;
                const matchesCity = state.filterCity === 'all' || b.baseCity === state.filterCity;
                return matchesType && matchesCity;
            });
        }
    },

    actions: {
        async fetchBranches() {
            this.isLoading = true;
            try {
                const { data } = await axios.get<BankMarker[]>('https://localhost:7148/api/branches');
                
                // Парсим JSON детали сразу при получении
                this.rawBranches = data.map(branch => ({
                    ...branch,
                    details: branch.dataJson ? JSON.parse(branch.dataJson) : null
                }));

            } catch (error) {
                console.error("Api error:", error);
            } finally {
                this.isLoading = false;
            }
        },

        // Метод для импорта JSON файла
        async importJson(file: File): Promise<boolean> {
            try {
                const formData = new FormData();
                formData.append('file', file);

                await axios.post(
                    'https://localhost:7148/api/branches/import-json',
                    formData
                );

                await this.fetchBranches();
                return true;
            } catch (err) {
                console.error("Ошибка импорта:", err);
                return false;
            }
        },

        async updateBranchStatus(id: number, isClosed: boolean): Promise<boolean>{
            try {
                await axios.patch(`https://localhost:7148/api/branches/${id}`, {
                    isTemporaryClosed: isClosed
                });
                const branch = this.rawBranches.find(b => b.id === id);

                if (branch) {
                    branch.isTemporaryClosed = isClosed;
                    
                    // Также обновляем внутри вложенных деталей (для Popup)
                    if (branch.details) {
                        branch.details!.IsTemporaryClosed = isClosed;
                    }
                }
                return true;
            } catch (error) {
                  console.error("Error updating branch status:", error);
                  return false;
            }
        }
    }
});