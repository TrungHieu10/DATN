/**
 * formatMetric.js - Helper functions định dạng chỉ số sức khỏe
 */

export const formatBMI = (weight, height) => {
  if (!weight || !height) return null;
  const bmi = weight / ((height / 100) ** 2);
  return bmi.toFixed(1);
};

export const formatBP = (systolic, diastolic) => {
  if (!systolic || !diastolic) return null;
  return `${systolic}/${diastolic} mmHg`;
};

export const formatBloodGlucose = (value) => {
  if (!value) return null;
  return `${value} mg/dL`;
};

export const formatCholesterol = (value) => {
  if (!value) return null;
  return `${value} mg/dL`;
};

export const formatHeartRate = (value) => {
  if (!value) return null;
  return `${value} bpm`;
};

export const formatTemperature = (value) => {
  if (!value) return null;
  return `${value}°C`;
};

export const formatPercentage = (value) => {
  if (value === null || value === undefined) return null;
  return `${(value * 100).toFixed(1)}%`;
};

export const formatDate = (date) => {
  if (!date) return null;
  return new Date(date).toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });
};

export const formatDateTime = (date) => {
  if (!date) return null;
  return new Date(date).toLocaleString('vi-VN');
};

export const getBMICategory = (bmi) => {
  if (bmi < 18.5) return 'Underweight';
  if (bmi < 25) return 'Normal';
  if (bmi < 30) return 'Overweight';
  return 'Obese';
};

export const getBloodPressureCategory = (systolic, diastolic) => {
  if (systolic < 120 && diastolic < 80) return 'Normal';
  if (systolic < 130 && diastolic < 80) return 'Elevated';
  if (systolic < 140 || diastolic < 90) return 'Stage 1 Hypertension';
  if (systolic >= 180 || diastolic >= 120) return 'Hypertensive Crisis';
  return 'Stage 2 Hypertension';
};

export const getBloodGlucoseCategory = (glucose) => {
  if (glucose < 70) return 'Low (Hypoglycemia)';
  if (glucose < 100) return 'Normal (Fasting)';
  if (glucose < 126) return 'Prediabetes';
  return 'Diabetes';
};

export default {
  formatBMI,
  formatBP,
  formatBloodGlucose,
  formatCholesterol,
  formatHeartRate,
  formatTemperature,
  formatPercentage,
  formatDate,
  formatDateTime,
  getBMICategory,
  getBloodPressureCategory,
  getBloodGlucoseCategory,
};
