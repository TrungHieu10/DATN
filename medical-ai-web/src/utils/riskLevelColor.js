/**
 * riskLevelColor.js - Ánh xạ mức độ rủi ro sang màu sắc
 */

export const getRiskLevelColor = (riskLevel) => {
  const colorMap = {
    low: 'success', // green
    medium: 'warning', // yellow
    high: 'danger', // red
  };
  return colorMap[riskLevel?.toLowerCase()] || 'default';
};

export const getRiskLevelStyle = (riskLevel) => {
  const styleMap = {
    low: { bg: '#dcfce7', text: '#166534', border: '#22c55e' },
    medium: { bg: '#fef3c7', text: '#92400e', border: '#f59e0b' },
    high: { bg: '#fee2e2', text: '#991b1b', border: '#ef4444' },
  };
  return styleMap[riskLevel?.toLowerCase()] || { bg: '#f3f4f6', text: '#374151', border: '#d1d5db' };
};

export const getRiskLevelLabel = (riskLevel) => {
  const labelMap = {
    low: '✓ Low Risk - Continue healthy habits',
    medium: '⚠️ Medium Risk - Take preventive measures',
    high: '🚨 High Risk - Consult healthcare provider',
  };
  return labelMap[riskLevel?.toLowerCase()] || 'Unknown Risk';
};

export const getProbabilityColor = (probability) => {
  if (probability < 0.3) return 'success';
  if (probability < 0.6) return 'warning';
  return 'danger';
};

export const getProbabilityLabel = (probability) => {
  if (probability < 0.3) return 'Low';
  if (probability < 0.6) return 'Medium';
  return 'High';
};

export default {
  getRiskLevelColor,
  getRiskLevelStyle,
  getRiskLevelLabel,
  getProbabilityColor,
  getProbabilityLabel,
};
