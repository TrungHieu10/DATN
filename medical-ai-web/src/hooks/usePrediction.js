import { useState, useCallback, useEffect } from 'react';
import predictionApi from '../api/predictionApi';

/**
 * Custom Hook - Quản lý kết quả dự đoán
 * Có cache để tránh fetch lặp lại
 * @param {string} checkupId
 * @returns { prediction, shapValues, loading, error, refetch }
 */
export const usePrediction = (checkupId) => {
  const [prediction, setPrediction] = useState(null);
  const [shapValues, setShapValues] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [cacheKey, setCacheKey] = useState(null);

  const fetchPrediction = useCallback(async (id) => {
    if (!id) return;

    // Kiểm tra cache
    const cached = sessionStorage.getItem(`prediction_${id}`);
    if (cached && cacheKey === id) {
      const data = JSON.parse(cached);
      setPrediction(data.prediction);
      setShapValues(data.shapValues);
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const [predictionData, shapData] = await Promise.all([
        predictionApi.getPredictionsByCheckupId(id),
        predictionApi.getShapValues(id),
      ]);

      setPrediction(predictionData);
      setShapValues(shapData);
      setCacheKey(id);

      // Cache result
      sessionStorage.setItem(
        `prediction_${id}`,
        JSON.stringify({
          prediction: predictionData,
          shapValues: shapData,
        })
      );
    } catch (err) {
      setError(err.message || 'Failed to fetch prediction');
    } finally {
      setLoading(false);
    }
  }, [cacheKey]);

  useEffect(() => {
    fetchPrediction(checkupId);
  }, [checkupId, fetchPrediction]);

  const refetch = useCallback(() => {
    setCacheKey(null);
    fetchPrediction(checkupId);
  }, [checkupId, fetchPrediction]);

  return {
    prediction,
    shapValues,
    loading,
    error,
    refetch,
  };
};

export default usePrediction;
