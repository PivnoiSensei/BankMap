import { defineStore } from 'pinia'
import axios from 'axios'

export interface Branch {
    id: number;
    name: string;
    address: string;
    latitude: number;
    longitude: number;
    status: string;
}

export const useBranchStore = defineStore('branchStore', {
    state: () => ({
        branches: [] as Branch[],
        isLoading: false,
        draftCoords: { lat: 50.4501, lng: 30.5234 },
        selectedBranch: null as Branch | null,
    }),
    actions: {
        async fetchBranches() {
            this.isLoading = true;
            try{
                const {data} = await axios.get<Branch[]>('https://localhost:7148/api/branches');
                this.branches = data;
            }catch(error){
                console.error("Api error: ", error);
            }finally{
                this.isLoading = false;
            }
        },
        async addBranch(newBranch: Omit<Branch, 'id'>){
            try{
                const {data} = await axios.post<Branch>('https://localhost:7148/api/branches', newBranch)
                this.branches.push(data);
                return true;
            }catch(error){
                console.error("Error occured while adding new branch: ", error);
                return false;
            }
        },
        async updateBranch(branch: Branch){
            try {
                await axios.put(`https://localhost:7148/api/branches/${branch.id}`, branch);
                const index = this.branches.findIndex(b => b.id === branch.id)

                if(index !== -1){
                    this.branches[index] = {...branch};
                }

                return true;
            } catch (error) {
                console.error("Error while updating branch data: ", error);
                return false;
            }
        },
        async deleteBranch(id: number){
            try {
                await axios.delete(`https://localhost:7148/api/branches/${id}`);
                this.branches = this.branches.filter(b => b.id !== id);
                return true;
            } catch (error) {
                console.error("Error while deleting branch data: ", error);
                return false;
            }
        },
        async importCsv(file: File) {
            const formData = new FormData();
            formData.append('file', file);
            try {
                await axios.post('https://localhost:7148/api/branches/import-csv', formData, {
                    headers: { 'Content-Type': 'multipart/form-data' }
                });
                await this.fetchBranches();
                return true;
            } catch (error) {
                console.error("Помилка імпорту CSV:", error);
                return false;
            }
        },
    }
})