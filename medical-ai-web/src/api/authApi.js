import axiosClient from './axiosClient';

/**
 * Auth API - Quản lý authentication
 */
const authApi = {
  /**
   * Đăng nhập
   * @param {string} email
   * @param {string} password
   * @returns {Promise} { accessToken, refreshToken, user }
   */
  login: async (email, password) => {
    try {
      const response = await axiosClient.post('/auth/login', { email, password });
      const { accessToken, refreshToken, user } = response.data;
      
      // Lưu tokens vào localStorage
      localStorage.setItem('accessToken', accessToken);
      localStorage.setItem('refreshToken', refreshToken);
      
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Login failed' };
    }
  },

  /**
   * Đăng ký tài khoản
   * @param {string} email
   * @param {string} password
   * @param {string} fullName
   * @returns {Promise} { message, user }
   */
  register: async (email, password, fullName) => {
    try {
      const response = await axiosClient.post('/auth/register', {
        email,
        password,
        fullName,
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Register failed' };
    }
  },

  /**
   * Refresh access token
   * @returns {Promise} { accessToken, refreshToken }
   */
  refreshToken: async () => {
    try {
      const refreshToken = localStorage.getItem('refreshToken');
      if (!refreshToken) {
        throw new Error('No refresh token available');
      }

      const response = await axiosClient.post('/auth/refresh-token', { refreshToken });
      const { accessToken, newRefreshToken } = response.data;

      localStorage.setItem('accessToken', accessToken);
      if (newRefreshToken) {
        localStorage.setItem('refreshToken', newRefreshToken);
      }

      return response.data;
    } catch (error) {
      // Xóa tokens nếu refresh thất bại
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      throw error.response?.data || { message: 'Token refresh failed' };
    }
  },

  /**
   * Đăng xuất
   */
  logout: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  },

  /**
   * Verify token còn hạn
   * @returns {boolean}
   */
  isAuthenticated: () => {
    const token = localStorage.getItem('accessToken');
    return !!token;
  },

  /**
   * Lấy thông tin người dùng hiện tại
   * @returns {Promise}
   */
  getCurrentUser: async () => {
    try {
      const response = await axiosClient.get('/auth/me');
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Failed to fetch user' };
    }
  },
};

export default authApi;
