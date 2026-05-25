/**
 * DashboardPage - Trang tổng quan sức khỏe
 */
import { useCheckupHistory } from '../hooks/useCheckupHistory';
import { usePredictionHistory } from '../hooks/usePredictionHistory';
import HealthTrendChart from '../components/charts/HealthTrendChart';
import Card from '../components/ui/Card';
import Badge from '../components/ui/Badge';
import Button from '../components/ui/Button';
import Spinner from '../components/ui/Spinner';
import { Link } from 'react-router-dom';

export const DashboardPage = () => {
  const { checkups, loading: checkupLoading, total } = useCheckupHistory();
  const { predictions, loading: predictionsLoading } = usePredictionHistory();

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-blue-600 to-blue-800 text-white p-6 rounded-lg shadow-lg">
        <h1 className="text-3xl font-bold mb-2">Health Dashboard</h1>
        <p className="text-blue-100">Monitor your health metrics and AI predictions</p>
      </div>

      {/* Quick Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card shadow="md">
          <div className="text-center">
            <p className="text-gray-600 text-sm">Total Checkups</p>
            <p className="text-3xl font-bold text-blue-600">{total}</p>
          </div>
        </Card>

        <Card shadow="md">
          <div className="text-center">
            <p className="text-gray-600 text-sm">Recent Risk</p>
            <p className="text-3xl font-bold text-orange-600">
              {predictions[0]?.riskScore
                ? (predictions[0].riskScore * 100).toFixed(1)
                : 'N/A'}
              %
            </p>
          </div>
        </Card>

        <Card shadow="md">
          <div className="text-center">
            <p className="text-gray-600 text-sm">Last Checkup</p>
            <p className="text-lg font-semibold text-gray-800">
              {checkups[0]
                ? new Date(checkups[0].createdAt).toLocaleDateString()
                : 'Never'}
            </p>
          </div>
        </Card>

        <Card shadow="md">
          <div className="text-center">
            <Link to="/checkup">
              <Button size="sm" variant="primary" className="w-full">
                New Checkup
              </Button>
            </Link>
          </div>
        </Card>
      </div>

      {/* Health Trend */}
      <HealthTrendChart data={predictions} loading={predictionsLoading} />

      {/* Recent Checkups */}
      <Card header="Recent Checkups" shadow="lg">
        {checkupLoading ? (
          <Spinner />
        ) : checkups.length > 0 ? (
          <div className="space-y-3">
            {checkups.slice(0, 5).map((checkup) => (
              <Link
                key={checkup.id}
                to={`/result/${checkup.id}`}
                className="block p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors border border-gray-200"
              >
                <div className="flex items-center justify-between">
                  <div>
                    <p className="font-semibold text-gray-800">
                      {new Date(checkup.createdAt).toLocaleDateString()}
                    </p>
                    <p className="text-sm text-gray-600">
                      Checkup ID: {checkup.id}
                    </p>
                  </div>
                  <Badge variant="primary">View Results →</Badge>
                </div>
              </Link>
            ))}
          </div>
        ) : (
          <p className="text-center text-gray-600 py-8">
            No checkups yet.{' '}
            <Link to="/checkup" className="text-blue-600 hover:underline font-semibold">
              Create one now
            </Link>
          </p>
        )}
      </Card>

      {/* Health Tips */}
      <Card header="Health Tips" shadow="lg" className="bg-green-50">
        <ul className="space-y-3">
          <li className="flex items-start gap-3">
            <span className="text-2xl">💤</span>
            <div>
              <p className="font-semibold text-gray-800">Get Quality Sleep</p>
              <p className="text-sm text-gray-600">
                Aim for 7-9 hours daily for better health
              </p>
            </div>
          </li>
          <li className="flex items-start gap-3">
            <span className="text-2xl">🏃</span>
            <div>
              <p className="font-semibold text-gray-800">Regular Exercise</p>
              <p className="text-sm text-gray-600">
                150 minutes of moderate activity per week
              </p>
            </div>
          </li>
          <li className="flex items-start gap-3">
            <span className="text-2xl">🥗</span>
            <div>
              <p className="font-semibold text-gray-800">Balanced Diet</p>
              <p className="text-sm text-gray-600">
                Include fruits, vegetables, and whole grains
              </p>
            </div>
          </li>
        </ul>
      </Card>
    </div>
  );
};

export default DashboardPage;
