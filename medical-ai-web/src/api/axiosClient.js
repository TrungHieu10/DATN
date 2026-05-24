import axios from 'axios';

const axiosClient = axios.create({
    baseURL: 'https://localhost:5182/api', 
    headers: {
        'Content-Type': 'application/json',
    },
});

// Tự động đính kèm Token vào mọi Request
axiosClient.interceptors.request.use((config) => {
    const token = localStorage.getItem('jwt_token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export default axiosClient;