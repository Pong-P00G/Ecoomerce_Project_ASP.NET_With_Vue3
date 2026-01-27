import api from "./api";

export const dashboardApi = {
    async getAdminDashboard() {
        return await api.get("/admin/dashboard");
    },

    async getStats() {
        return await api.get("/admin/stats");
    }
};