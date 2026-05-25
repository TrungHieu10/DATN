import axiosClient from './axiosClient';

/**
 * Prediction API - Quản lý dự đoán AI
 */
const predictionApi = {
  /**
   * Lấy kết quả dự đoán theo checkupId
   * @param {string} checkupId
   * @returns {Promise} { predictions: [...], shapValues, riskLevel, advice }
   */
  getPredictionsByCheckupId: async (checkupId) => {
    try {
      const response = await axiosClient.get(`/clinical/${checkupId}/result`);
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Failed to fetch predictions' };
    }
  },

  /**
   * Lấy SHAP explanation values
   * @param {string} checkupId
   * @returns {Promise} { shapValues, feature_names, base_value }
   */
  getShapValues: async (checkupId) => {
    try {
      const response = await axiosClient.get(`/clinical/${checkupId}/shap`);
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Failed to fetch SHAP values' };
    }
  },

  /**
   * Lấy lịch sử dự đoán (health trend)
   * @param {Object} options - { limit, from, to }
   * @returns {Promise} [{ date, predictions, riskLevel }, ...]
   */
  getPredictionHistory: async (options = {}) => {
    try {
      const { limit = 10, from, to } = options;
      const params = { limit };
      
      if (from) params.from = from;
      if (to) params.to = to;

      const response = await axiosClient.get('/clinical/predictions/history', { params });
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Failed to fetch prediction history' };
    }
  },

  /**
   * Lấy mô hình đặc trưng (feature importance)
   * @param {string} diseaseType - e.g., 'diabetes', 'hypertension'
   * @returns {Promise} { features: [...], importance: [...] }
   */
  getFeatureImportance: async (diseaseType = 'all') => {
    try {
      const response = await axiosClient.get('/clinical/predictions/feature-importance', {
        params: { diseaseType },
      });
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Failed to fetch feature importance' };
    }
  },

  /**
   * Lấy thống kê risk level
   * @returns {Promise} { low: number, medium: number, high: number }
   */
  getRiskStatistics: async () => {
    try {
      const response = await axiosClient.get('/clinical/predictions/risk-stats');
      return response.data;
    } catch (error) {
      throw error.response?.data || { message: 'Failed to fetch risk statistics' };
    }
  },
};

export default predictionApi;
