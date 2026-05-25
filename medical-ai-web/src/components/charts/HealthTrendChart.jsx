/**
 * HealthTrendChart Component - Biểu đồ xu hướng sức khỏe
 * Sử dụng data từ usePredictionHistory hook
 */
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import Card from '../ui/Card';

export const HealthTrendChart = ({ data = [], loading = false }) => {
  if (loading) {
    return <Card header="Health Trend">Loading...</Card>;
  }

  if (!data || data.length === 0) {
    return <Card header="Health Trend">No data available</Card>;
  }

  return (
    <Card header="Health Trend Over Time" shadow="lg">
      <ResponsiveContainer width="100%" height={300}>
        <LineChart data={data}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="date" />
          <YAxis />
          <Tooltip />
          <Legend />
          <Line
            type="monotone"
            dataKey="riskScore"
            stroke="#ef4444"
            name="Risk Score"
            dot={false}
          />
          <Line
            type="monotone"
            dataKey="healthScore"
            stroke="#10b981"
            name="Health Score"
            dot={false}
          />
        </LineChart>
      </ResponsiveContainer>
    </Card>
  );
};

export default HealthTrendChart;
