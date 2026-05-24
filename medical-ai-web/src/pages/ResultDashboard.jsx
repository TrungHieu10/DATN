import { useLocation, useNavigate } from 'react-router-dom';
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, Cell } from 'recharts';
import { AlertTriangle, CheckCircle, Activity, BrainCircuit } from 'lucide-react';

export default function ResultDashboard() {
    const location = useLocation();
    const navigate = useNavigate();
    
    // Hứng dữ liệu mảng predictions từ trang Form truyền sang
    const predictions = location.state?.predictions || [];

    if (!predictions || predictions.length === 0) {
        return <div className="p-10 text-center">Đang tải dữ liệu báo cáo...</div>;
    }

    const isHealthy = predictions.length === 1 && predictions[0].DiseaseType === "Healthy";

    return (
        <div className="p-8 bg-gray-100 min-h-screen">
            <div className="max-w-7xl mx-auto">
                <div className="flex justify-between items-center mb-8">
                    <h1 className="text-3xl font-bold text-slate-800 flex items-center gap-3">
                        <BrainCircuit className="w-8 h-8 text-blue-600" /> Báo Cáo Y Khoa Tổng Hợp
                    </h1>
                    <button onClick={() => navigate('/clinical-form')} className="px-4 py-2 bg-slate-800 text-white rounded hover:bg-slate-700">
                        + Hồ sơ mới
                    </button>
                </div>

                {isHealthy ? (
                    <div className="bg-white p-10 rounded-2xl shadow-sm text-center border-t-4 border-green-500">
                        <CheckCircle className="w-20 h-20 text-green-500 mx-auto mb-4" />
                        <h2 className="text-2xl font-bold text-gray-800">Hoàn toàn bình thường</h2>
                        <p className="text-gray-500 mt-2">Bệnh nhân không có dấu hiệu rủi ro cao với 5 bệnh lý mục tiêu.</p>
                    </div>
                ) : (
                    <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                        {predictions.map((pred, idx) => {
                            // Parse chuỗi JSON
                            const shapData = JSON.parse(pred.shapValuesJSON || '[]');
                            const adviceData = JSON.parse(pred.adviceJSON || '{}');
                            
                            // Chuẩn bị data cho biểu đồ Recharts
                            const chartData = shapData.map(item => ({
                                name: item.feature,
                                value: item.impact,
                                color: item.impact > 0 ? '#ef4444' : '#22c55e' // Đỏ (gây bệnh), Xanh (bảo vệ)
                            })).reverse(); // Đảo ngược để vẽ từ trên xuống

                            return (
                                <div key={idx} className="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
                                    <div className="bg-red-50 p-4 border-b border-red-100 flex justify-between items-center">
                                        <h3 className="text-xl font-bold text-red-700 flex items-center gap-2">
                                            <AlertTriangle className="w-6 h-6" /> Nguy cơ: {pred.diseaseType}
                                        </h3>
                                        <span className="px-3 py-1 bg-red-600 text-white font-bold rounded-full text-sm">
                                            Xác suất: {(pred.probability * 100).toFixed(1)}%
                                        </span>
                                    </div>
                                    
                                    <div className="p-6">
                                        <div className="mb-6 bg-blue-50 border border-blue-100 p-4 rounded-lg">
                                            <h4 className="text-sm font-bold text-blue-800 uppercase mb-1">💡 Lời khuyên từ Đồ thị tri thức chuyên gia</h4>
                                            <p className="text-gray-700">{adviceData.vi}</p>
                                        </div>

                                        <h4 className="text-sm font-bold text-gray-500 uppercase mb-4">Phân tích tác nhân (Explainable AI)</h4>
                                        <div className="h-64 w-full">
                                            <ResponsiveContainer width="100%" height="100%">
                                                <BarChart data={chartData} layout="vertical" margin={{ top: 0, right: 30, left: 40, bottom: 0 }}>
                                                    <XAxis type="number" />
                                                    <YAxis dataKey="name" type="category" width={100} tick={{fontSize: 12, fill: '#4b5563'}} />
                                                    <Tooltip cursor={{fill: 'transparent'}} />
                                                    <Bar dataKey="value" radius={[0, 4, 4, 0]}>
                                                        {chartData.map((entry, index) => (
                                                            <Cell key={`cell-${index}`} fill={entry.color} />
                                                        ))}
                                                    </Bar>
                                                </BarChart>
                                            </ResponsiveContainer>
                                        </div>
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                )}
            </div>
        </div>
    );
}