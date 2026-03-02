import { defineStore } from 'pinia'
import axios from 'axios'
const API_URL = import.meta.env.VITE_API_URL || 'http://backend:8080';

//Full data structure from parsed JSON from DB 
export interface Break {
    from: string;
    to: string;
}

export interface WorkDay {
    dayOfWeek: string;
    from: string;
    to: string;
    breaks: Break[];
}

export interface Schedule {
    workStation: string;
    days: WorkDay[];
}

export interface CashDesk{
    externalId: number;
    description: string;
    workDays: WorkDay[];
}

export interface Branch {
    id: number;
    name: string;
    type: 'Department' | 'Atm' | 'Terminal';
    isTemporaryClosed: boolean;
    isRegular: boolean;
    address: {
        city: string;
        fullAddress: string;
        detailedAddress: string;
        latitude: number;
        longitude: number;
    };
    schedules: Schedule[];
    phones: {
        operatorCode: string;
        number: string;
        fullNumber: string;
    }[];
    cashDesks: CashDesk[];
}

export const useBranchStore = defineStore('branchStore', {
    state: () => ({
        branches: [] as Branch[],
        isLoading: false,
        selectedBranch: null as Branch | null,
        filterType: 'all', 
        filterCity: 'all'
    }),

    getters: {
        uniqueCities: (state) => {
            const cities = state.branches.map(b => b.address.city);
            return ['all', ...new Set(cities)].sort();
        },

        filteredBranches: (state) => {
            return state.branches.filter(b => {
                const matchesType = state.filterType === 'all' || b.type === state.filterType;
                const matchesCity = state.filterCity === 'all' || b.address.city === state.filterCity;
                return matchesType && matchesCity;
            });
        }
    },

    actions: {
        async fetchBranches() {
            this.isLoading = true;
            try {
                const { data } = await axios.get<Branch[]>(`${API_URL}/api/branches`);
                this.branches = data;
            } catch (error) {
                console.error("Api error:", error);
            } finally {
                this.isLoading = false;
            }
        },

        //Import JSON file for test
        async importJson(file: File): Promise<boolean> {
            try {
                const formData = new FormData();
                formData.append('file', file);

                await axios.post(
                    `${API_URL}/api/branches/import-json`,
                    formData
                );

                await this.fetchBranches();
                return true;
            } catch (err) {
                console.error("Ошибка импорта:", err);
                return false;
            }
        },

        // async updateBranchStatus(id: number, isClosed: boolean): Promise<boolean>{
        //     try {
        //         await axios.patch(`${API_URL}/api/branches/${id}`, {
        //             isTemporaryClosed: isClosed
        //         });
        //         const branch = this.rawBranches.find(b => b.id === id);

        //         if (branch) {
        //             branch.isTemporaryClosed = isClosed;
                    
        //             //Update details inside Popup
        //             if (branch.details) {
        //                 branch.details!.IsTemporaryClosed = isClosed;
        //             }
        //         }
        //         return true;
        //     } catch (error) {
        //           console.error("Error updating branch status:", error);
        //           return false;
        //     }
        // }
    }
});