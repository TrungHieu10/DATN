/**
 * ShapWaterfallChart Component - Biểu đồ SHAP (feature importance)
 * Bar chart hiển thị mức độ ảnh hưởng của từng đặc trưng
 */
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts';
import Card from '../ui/Card';

export const ShapWaterfallChart = ({ shapValues = [], loading = false }) => {
  if (loading) {
    return <Card header="SHAP Feature Importance">Loading...</Card>;
  }

  if (!shapValues || shapValues.length === 0) {
    return <Card header="SHAP Feature Importance">No data available</Card>;
  }

  // Chuyển đổi dữ liệu để hiển thị
  const chartData = shapValues.slice(0, 10).map((item) => ({
    feature: item.feature_name || item.name,
    importance: Math.abs(item.shap_value || item.value),
    impact: item.shap_value > 0 ? 'Positive' : 'Negative',
  }));

  return (
    <Card header="SHAP Waterfall Chart - Feature Importance" shadow="lg">
      <ResponsiveContainer width="100%" height={350}>
        <BarChart
          data={chartData}
          layout="vertical"
          margin={{ top: 5, right: 30, left: 200, bottom: 5 }}
        >
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis type="number" />
          <YAxis dataKey="feature" type="category" width={180} />
          <Tooltip formatter={(value) => value.toFixed(4)} />
          <Legend />
          <Bar dataKey="importance" fill="#3b82f6" name="Importance Score" />
        </BarChart>
      </ResponsiveContainer>

      {/* Base Value Info */}
      <div className="mt-4 p-4 bg-blue-50 rounded-lg text-sm text-gray-700">
        <p>
          <strong>Interpretation:</strong> Bar length indicates feature impact magnitude. Longer bars = stronger influence on prediction.
        </p>
      </div>
    </Card>
  );
};

export default ShapWaterfallChart;
