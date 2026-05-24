import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axiosClient from '../api/axiosClient';

export default function ClinicalForm() {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [formData, setFormData] = useState({
        location: "Khoa Khám Bệnh", notes: "",
        height_cm: 165, weight_kg: 70, systolicBP: 120, diastolicBP: 80,
        bloodGlucose: 100, hbA1c: 5.5, cholesterol_Total: 180,
        serumCreatinine: 0.9, bloodUrea: 30, albumin_Urine: 0, sugar_Urine: 0,
        alt_SGPT: 25, ast_SGOT: 25, totalBilirubin: 0.8, directBilirubin: 0.2, hemoglobin: 14,
        smokingStatus: 0, alcoholConsumption: false, physicalActivity: true,
        hypertension_History: false, heartDisease_History: false, everMarried: true, workType: 2, residenceType: 1
    });

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        setFormData({
            ...formData,
            [name]: type === 'checkbox' ? checked : (type === 'number' ? Number(value) : value)
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        try {
            // Gửi sang C# API (API sẽ tự gọi Python và lưu DB)
            const response = await axiosClient.post('/Clinical/submit', formData);
            // Chuyển hướng sang màn hình Dashboard với dữ liệu trả về
            navigate('/result/latest', { state: { predictions: response.data } });
        } catch (error) {
            alert("Lỗi khi phân tích: " + (error.response?.data || error.message));
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="p-6 bg-gray-50 min-h-screen">
            <div className="max-w-6xl mx-auto bg-white p-8 rounded-xl shadow-md">
                <h1 className="text-2xl font-bold text-gray-800 mb-6 border-b pb-4">🩺 Hồ Sơ Khám Lâm Sàng</h1>
                
                <form onSubmit={handleSubmit}>
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                        {/* CỘT 1: SINH HIỆU */}
                        <div className="space-y-4">
                            <h2 className="text-lg font-semibold text-blue-600 bg-blue-50 p-2 rounded">1. Sinh hiệu cơ bản</h2>
                            <div>
                                <label className="block text-sm font-medium text-gray-700">Huyết áp Tâm thu (mmHg)</label>
                                <input type="number" name="systolicBP" value={formData.systolicBP} onChange={handleChange} className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 p-2 border" required />
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700">Huyết áp Tâm trương (mmHg)</label>
                                <input type="number" name="diastolicBP" value={formData.diastolicBP} onChange={handleChange} className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 p-2 border" required />
                            </div>
                            <div className="flex gap-4">
                                <div className="w-1/2">
                                    <label className="block text-sm font-medium text-gray-700">Chiều cao (cm)</label>
                                    <input type="number" name="height_cm" value={formData.height_cm} onChange={handleChange} className="mt-1 block w-full rounded-md border-gray-300 shadow-sm p-2 border" />
                                </div>
                                <div className="w-1/2">
                                    <label className="block text-sm font-medium text-gray-700">Cân nặng (kg)</label>
                                    <input type="number" name="weight_kg" value={formData.weight_kg} onChange={handleChange} className="mt-1 block w-full rounded-md border-gray-300 shadow-sm p-2 border" />
                                </div>
                            </div>
                        </div>

                        {/* CỘT 2: XÉT NGHIỆM MÁU */}
                        <div className="space-y-4">
                            <h2 className="text-lg font-semibold text-red-600 bg-red-50 p-2 rounded">2. Chỉ số Sinh hóa</h2>
                            <div>
                                <label className="block text-sm font-medium text-gray-700">Đường huyết (mg/dL)</label>
                                <input type="number" step="0.1" name="bloodGlucose" value={formData.bloodGlucose} onChange={handleChange} className="mt-1 block w-full rounded-md border-gray-300 shadow-sm p-2 border" />
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700">HbA1c (%)</label>
                                <input type="number" step="0.1" name="hbA1c" value={formData.hbA1c} onChange={handleChange} className="mt-1 block w-full rounded-md border-gray-300 shadow-sm p-2 border" />
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700">Men gan ALT (U/L)</label>
                                <input type="number" step="0.1" name="alt_SGPT" value={formData.alt_SGPT} onChange={handleChange} className="mt-1 block w-full rounded-md border-gray-300 shadow-sm p-2 border" />
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700">Creatinine huyết thanh (mg/dL)</label>
                                <input type="number" step="0.1" name="serumCreatinine" value={formData.serumCreatinine} onChange={handleChange} className="mt-1 block w-full rounded-md border-gray-300 shadow-sm p-2 border" />
                            </div>
                        </div>

                        {/* CỘT 3: LỐI SỐNG & TIỀN SỬ */}
                        <div className="space-y-4">
                            <h2 className="text-lg font-semibold text-green-600 bg-green-50 p-2 rounded">3. Tiền sử & Lối sống</h2>
                            <label className="flex items-center space-x-3 p-2 bg-gray-50 rounded border">
                                <input type="checkbox" name="hypertension_History" checked={formData.hypertension_History} onChange={handleChange} className="h-5 w-5 text-blue-600 rounded" />
                                <span className="text-sm font-medium text-gray-700">Tiền sử Cao huyết áp</span>
                            </label>
                            <label className="flex items-center space-x-3 p-2 bg-gray-50 rounded border">
                                <input type="checkbox" name="alcoholConsumption" checked={formData.alcoholConsumption} onChange={handleChange} className="h-5 w-5 text-blue-600 rounded" />
                                <span className="text-sm font-medium text-gray-700">Thường xuyên uống rượu bia</span>
                            </label>
                            <div>
                                <label className="block text-sm font-medium text-gray-700">Ghi chú lâm sàng</label>
                                <textarea name="notes" value={formData.notes} onChange={handleChange} rows="3" className="mt-1 block w-full rounded-md border-gray-300 shadow-sm p-2 border"></textarea>
                            </div>
                        </div>
                    </div>

                    <div className="mt-8 flex justify-end">
                        <button type="submit" disabled={loading} className={`px-8 py-3 rounded-lg font-bold text-white transition-all ${loading ? 'bg-gray-400' : 'bg-blue-600 hover:bg-blue-700 shadow-lg'}`}>
                            {loading ? '⏳ Hệ thống AI đang phân tích...' : '🤖 Chẩn Đoán Đa Bệnh Lý'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}