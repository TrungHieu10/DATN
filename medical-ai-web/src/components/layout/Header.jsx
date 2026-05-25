/**
 * Header Component - Top navigation
 */
import { Link } from 'react-router-dom';
import useAuth from '../../hooks/useAuth';

export const Header = () => {
  const { user, logout } = useAuth();

  return (
    <header className="bg-blue-600 text-white shadow-lg">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <Link to="/" className="text-2xl font-bold">
            🏥 MedicalAI
          </Link>

          {/* Right Menu */}
          <div className="flex items-center gap-4">
            {user && (
              <>
                <span className="text-sm">Welcome, {user.fullName || user.email}</span>
                <button
                  onClick={logout}
                  className="bg-blue-700 hover:bg-blue-800 px-4 py-2 rounded-lg transition-colors"
                >
                  Logout
                </button>
              </>
            )}
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
