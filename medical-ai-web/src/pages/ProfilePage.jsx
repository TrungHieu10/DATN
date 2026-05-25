/**
 * ProfilePage - Thông tin cá nhân & lịch sử timeline
 */
import useAuth from '../hooks/useAuth';
import { useCheckupHistory } from '../hooks/useCheckupHistory';
import Card from '../components/ui/Card';
import Button from '../components/ui/Button';
import Spinner from '../components/ui/Spinner';
import { Link } from 'react-router-dom';

export const ProfilePage = () => {
  const { user, logout } = useAuth();
  const { checkups, loading } = useCheckupHistory();

  const handleLogout = () => {
    logout();
    window.location.href = '/login';
  };

  return (
    <div className="max-w-4xl mx-auto space-y-6">
      {/* User Info Card */}
      <Card header="Personal Information" shadow="lg">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <p className="text-gray-600 text-sm mb-1">Full Name</p>
            <p className="text-2xl font-semibold text-gray-800">
              {user?.fullName || user?.email}
            </p>
          </div>
          <div>
            <p className="text-gray-600 text-sm mb-1">Email</p>
            <p className="text-lg text-gray-800">{user?.email}</p>
          </div>
          <div>
            <p className="text-gray-600 text-sm mb-1">Member Since</p>
            <p className="text-lg text-gray-800">
              {user?.createdAt
                ? new Date(user.createdAt).toLocaleDateString()
                : 'Unknown'}
            </p>
          </div>
          <div>
            <p className="text-gray-600 text-sm mb-1">Total Checkups</p>
            <p className="text-lg text-gray-800">{checkups.length}</p>
          </div>
        </div>

        <div className="mt-6 flex gap-3">
          <Button variant="outline">Edit Profile</Button>
          <Button variant="danger" onClick={handleLogout}>
            Logout
          </Button>
        </div>
      </Card>

      {/* Checkup Timeline */}
      <Card header="Checkup History Timeline" shadow="lg">
        {loading ? (
          <div className="flex justify-center py-8">
            <Spinner />
          </div>
        ) : checkups.length > 0 ? (
          <div className="space-y-4">
            {checkups.map((checkup, idx) => (
              <div key={checkup.id} className="relative">
                {/* Timeline dot */}
                <div className="absolute left-0 top-2 w-4 h-4 bg-blue-600 rounded-full border-4 border-white -ml-6"></div>

                {/* Timeline item */}
                <Link
                  to={`/result/${checkup.id}`}
                  className="block ml-4 p-4 bg-gray-50 rounded-lg hover:bg-blue-50 transition-colors border border-gray-200 cursor-pointer"
                >
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="font-semibold text-gray-800">
                        Checkup #{checkups.length - idx}
                      </p>
                      <p className="text-sm text-gray-600">
                        {new Date(checkup.createdAt).toLocaleString()}
                      </p>
                      <p className="text-xs text-gray-500 mt-1">
                        ID: {checkup.id}
                      </p>
                    </div>
                    <div className="text-right">
                      <p className="text-sm font-semibold text-blue-600">
                        View Details →
                      </p>
                    </div>
                  </div>
                </Link>

                {/* Timeline connector */}
                {idx < checkups.length - 1 && (
                  <div className="absolute left-0 top-8 w-0.5 h-12 bg-gray-300 -ml-4"></div>
                )}
              </div>
            ))}
          </div>
        ) : (
          <div className="text-center py-8 text-gray-600">
            <p>No checkups yet.</p>
            <Link
              to="/checkup"
              className="text-blue-600 hover:underline font-semibold mt-2 inline-block"
            >
              Create your first checkup →
            </Link>
          </div>
        )}
      </Card>

      {/* Health Stats */}
      <Card header="Your Health Statistics" shadow="lg" className="bg-blue-50">
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <div className="text-center p-4 bg-white rounded-lg">
            <p className="text-gray-600 text-sm">Avg Risk Score</p>
            <p className="text-2xl font-bold text-orange-600">
              {checkups.length > 0 ? 'N/A' : '-'}
            </p>
          </div>
          <div className="text-center p-4 bg-white rounded-lg">
            <p className="text-gray-600 text-sm">High Risk</p>
            <p className="text-2xl font-bold text-red-600">0</p>
          </div>
          <div className="text-center p-4 bg-white rounded-lg">
            <p className="text-gray-600 text-sm">Medium Risk</p>
            <p className="text-2xl font-bold text-yellow-600">0</p>
          </div>
          <div className="text-center p-4 bg-white rounded-lg">
            <p className="text-gray-600 text-sm">Low Risk</p>
            <p className="text-2xl font-bold text-green-600">0</p>
          </div>
        </div>
      </Card>
    </div>
  );
};

export default ProfilePage;
