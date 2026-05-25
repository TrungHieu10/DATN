/**
 * ResultDashboard - Hiển thị kết quả dự đoán AI
 */
import { useParams } from 'react-router-dom';
import usePrediction from '../hooks/usePrediction';
import ShapWaterfallChart from '../components/charts/ShapWaterfallChart';
import RiskGaugeChart from '../components/charts/RiskGaugeChart';
import Card from '../components/ui/Card';
import Badge from '../components/ui/Badge';
import Spinner from '../components/ui/Spinner';
import { getRiskLevelColor } from '../utils/riskLevelColor';

export const ResultDashboard = () => {
  const { checkupId } = useParams();
  const { prediction, shapValues, loading, error } = usePrediction(checkupId);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <Spinner />
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-4xl mx-auto p-6">
        <div className="bg-red-50 border border-red-200 text-red-700 p-4 rounded-lg">
          <h2 className="font-bold mb-2">Error Loading Results</h2>
          <p>{error}</p>
        </div>
      </div>
    );
  }

  if (!prediction) {
    return <div className="text-center p-6">No prediction data available</div>;
  }

  const riskColor = getRiskLevelColor(prediction.riskLevel);

  return (
    <div className="max-w-6xl mx-auto space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-blue-600 to-blue-800 text-white p-6 rounded-lg shadow-lg">
        <h1 className="text-3xl font-bold mb-2">Health Analysis Results</h1>
        <p className="text-blue-100">Checkup ID: {checkupId}</p>
      </div>

      {/* Risk Summary Card */}
      <Card shadow="lg" className="bg-gradient-to-r from-blue-50 to-blue-100">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="text-center">
            <p className="text-gray-600 mb-2">Overall Risk Level</p>
            <Badge variant={riskColor} className="text-xl px-4 py-2">
              {prediction.riskLevel?.toUpperCase()}
            </Badge>
          </div>

          <div className="text-center">
            <p className="text-gray-600 mb-2">Risk Score</p>
            <p className="text-4xl font-bold text-blue-600">
              {(prediction.riskScore * 100).toFixed(1)}%
            </p>
          </div>

          <div className="text-center">
            <p className="text-gray-600 mb-2">Analysis Date</p>
            <p className="text-2xl font-semibold text-gray-800">
              {new Date(prediction.createdAt).toLocaleDateString()}
            </p>
          </div>
        </div>
      </Card>

      {/* Risk Gauge Chart */}
      <RiskGaugeChart
        riskLevel={prediction.riskLevel}
        probability={prediction.riskScore * 100}
        predictions={prediction.predictions}
      />

      {/* Disease Predictions */}
      <Card header="Disease Predictions" shadow="lg">
        <div className="space-y-3">
          {prediction.predictions && prediction.predictions.map((pred, idx) => (
            <div
              key={idx}
              className="flex items-center justify-between p-4 bg-gray-50 rounded-lg border border-gray-200"
            >
              <div>
                <h4 className="font-semibold text-gray-800">{pred.disease}</h4>
                <p className="text-sm text-gray-600">{pred.description}</p>
              </div>
              <div className="text-right">
                <p className="text-2xl font-bold text-blue-600">
                  {(pred.probability * 100).toFixed(1)}%
                </p>
                <Badge
                  variant={
                    pred.probability > 0.7
                      ? 'danger'
                      : pred.probability > 0.4
                      ? 'warning'
                      : 'success'
                  }
                >
                  {pred.probability > 0.7
                    ? 'High'
                    : pred.probability > 0.4
                    ? 'Medium'
                    : 'Low'}
                </Badge>
              </div>
            </div>
          ))}
        </div>
      </Card>

      {/* SHAP Feature Importance */}
      <ShapWaterfallChart shapValues={shapValues} loading={loading} />

      {/* Health Advice */}
      <Card
        header="Personalized Recommendations"
        shadow="lg"
        className="bg-green-50 border-l-4 border-green-500"
      >
        <div className="space-y-4">
          {prediction.advice && prediction.advice.length > 0 ? (
            prediction.advice.map((advice, idx) => (
              <div key={idx} className="flex items-start gap-3">
                <span className="text-2xl">💡</span>
                <p className="text-gray-700">{advice}</p>
              </div>
            ))
          ) : (
            <p className="text-gray-600">No specific recommendations at this time.</p>
          )}
        </div>
      </Card>

      {/* Next Steps */}
      <Card header="Next Steps" shadow="lg" className="bg-blue-50">
        <ul className="space-y-2 text-gray-700">
          <li>✓ Review your health metrics regularly</li>
          <li>✓ Follow personalized recommendations</li>
          <li>✓ Schedule follow-up with healthcare provider if needed</li>
          <li>✓ Track your health metrics over time</li>
        </ul>
      </Card>
    </div>
  );
};

export default ResultDashboard;