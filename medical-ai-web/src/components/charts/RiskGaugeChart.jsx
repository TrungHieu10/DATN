/**
 * RiskGaugeChart Component - Biểu đồ mức độ rủi ro
 * Hiển thị % xác suất mắc bệnh
 */
import { PieChart, Pie, Cell, ResponsiveContainer, Legend, Tooltip } from 'recharts';
import Card from '../ui/Card';

export const RiskGaugeChart = ({ riskLevel = 'medium', probability = 50, predictions = [] }) => {
  // Tính toán dữ liệu cho pie chart
  const data = [
    { name: 'Risk', value: probability },
    { name: 'Safe', value: 100 - probability },
  ];

  const riskColors = {
    low: '#10b981', // green
    medium: '#f59e0b', // amber
    high: '#ef4444', // red
  };

  const color = riskColors[riskLevel] || riskColors.medium;

  return (
    <Card header="Risk Assessment" shadow="lg">
      <div className="flex flex-col items-center">
        <ResponsiveContainer width="100%" height={250}>
          <PieChart>
            <Pie
              data={data}
              cx="50%"
              cy="50%"
              startAngle={180}
              endAngle={0}
              innerRadius={60}
              outerRadius={100}
              paddingAngle={2}
              dataKey="value"
            >
              <Cell fill={color} />
              <Cell fill="#e5e7eb" />
            </Pie>
            <Tooltip formatter={(value) => `${value}%`} />
          </PieChart>
        </ResponsiveContainer>

        {/* Risk Label */}
        <div className="text-center mt-4">
          <p className="text-2xl font-bold" style={{ color }}>
            {probability}%
          </p>
          <p className="text-sm text-gray-600">
            Risk Level: <span className="font-semibold uppercase">{riskLevel}</span>
          </p>
        </div>

        {/* Predictions List */}
        {predictions && predictions.length > 0 && (
          <div className="mt-6 w-full">
            <h4 className="text-sm font-semibold text-gray-700 mb-2">Predictions:</h4>
            <ul className="space-y-2">
              {predictions.map((pred, idx) => (
                <li key={idx} className="text-sm text-gray-600">
                  • {pred.disease}: {(pred.probability * 100).toFixed(1)}%
                </li>
              ))}
            </ul>
          </div>
        )}
      </div>
    </Card>
  );
};

export default RiskGaugeChart;
