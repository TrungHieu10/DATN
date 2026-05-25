import axiosClient from './axiosClient';

/**
 * Clinical API - Quản lý khám sức khỏe
 */
const clinicalApi = {
  /**
   * Submit checkup mới
   * @param {Object} checkupData - { metrics: {...}, userId }
   * @returns {Promise} { checkupId, predictions, shapValues }
   */
  submitCheckup: async (checkupData) => {
    try {
      const response = await axiosClient.post('/clinical/submit', checkupData);
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Failed to submit checkup' };
    }
  },

  /**
   * Lấy chi tiết khám theo ID
   * @param {string} checkupId
   * @returns {Promise}
   */
  getCheckupById: async (checkupId) => {
    try {
      const response = await axiosClient.get(`/clinical/${checkupId}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Failed to fetch checkup' };
    }
  },

  /**
   * Lấy lịch sử khám của người dùng
   * @param {Object} options - { page, pageSize, sortBy }
   * @returns {Promise} { items, total, page, pageSize }
   */
  getHistory: async (options = {}) => {
    try {
      const { page = 1, pageSize = 10, sortBy = 'createdAt' } = options;
      const response = await axiosClient.get('/clinical/history', {
        params: {
          page,
          pageSize,
          sortBy,
        },
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Failed to fetch history' };
    }
  },

  /**
   * Lấy tất cả khám của user hiện tại
   * @returns {Promise}
   */
  getMyCheckups: async () => {
    try {
      const response = await axiosClient.get('/clinical/my-checkups');
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Failed to fetch your checkups' };
    }
  },

  /**
   * Cập nhật khám
   * @param {string} checkupId
   * @param {Object} updateData
   * @returns {Promise}
   */
  updateCheckup: async (checkupId, updateData) => {
    try {
      const response = await axiosClient.put(`/clinical/${checkupId}`, updateData);
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Failed to update checkup' };
    }
  },

  /**
   * Xóa khám
   * @param {string} checkupId
   * @returns {Promise}
   */
  deleteCheckup: async (checkupId) => {
    try {
      const response = await axiosClient.delete(`/clinical/${checkupId}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Failed to delete checkup' };
    }
  },
};

export default clinicalApi;
