/**
 * ClinicalForm - Multi-step form nhập dữ liệu khám sức khỏe
 */
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import FormStepper from '../components/forms/FormStepper';
import MetricInputGroup from '../components/forms/MetricInputGroup';
import LifestyleSection from '../components/forms/LifestyleSection';
import Button from '../components/ui/Button';
import clinicalApi from '../api/clinicalApi';

export const ClinicalForm = () => {
  const navigate = useNavigate();
  const [currentStep, setCurrentStep] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const [formData, setFormData] = useState({
    // Basic Metrics
    age: '',
    bloodPressureSystolic: '',
    bloodPressureDiastolic: '',
    heartRate: '',
    temperature: '',
    weight: '',
    height: '',
    bmi: '',

    // Blood Tests
    bloodGlucose: '',
    totalCholesterol: '',
    ldl: '',
    hdl: '',
    triglycerides: '',

    // Lifestyle
    sleepHours: '',
    exerciseMinutes: '',
    stressLevel: '',
    smokingStatus: '',
    alcoholConsumption: '',
    fruitsVegetablesServings: '',
  });

  const basicMetrics = [
    { name: 'age', label: 'Age', placeholder: 'e.g., 45', required: true },
    { name: 'bloodPressureSystolic', label: 'BP Systolic (mmHg)', placeholder: 'e.g., 120', required: true },
    { name: 'bloodPressureDiastolic', label: 'BP Diastolic (mmHg)', placeholder: 'e.g., 80', required: true },
    { name: 'heartRate', label: 'Heart Rate (bpm)', placeholder: 'e.g., 72', required: true },
    { name: 'temperature', label: 'Temperature (°C)', placeholder: 'e.g., 36.5', required: true },
    { name: 'weight', label: 'Weight (kg)', placeholder: 'e.g., 70', required: true },
    { name: 'height', label: 'Height (cm)', placeholder: 'e.g., 170', required: true },
  ];

  const bloodTestMetrics = [
    { name: 'bloodGlucose', label: 'Blood Glucose (mg/dL)', placeholder: 'e.g., 100', required: true },
    { name: 'totalCholesterol', label: 'Total Cholesterol (mg/dL)', placeholder: 'e.g., 200', required: true },
    { name: 'ldl', label: 'LDL (mg/dL)', placeholder: 'e.g., 130', required: true },
    { name: 'hdl', label: 'HDL (mg/dL)', placeholder: 'e.g., 40', required: true },
    { name: 'triglycerides', label: 'Triglycerides (mg/dL)', placeholder: 'e.g., 150', required: true },
  ];

  const steps = ['Basic Metrics', 'Blood Tests', 'Lifestyle', 'Review & Submit'];

  const handleInputChange = (name, value) => {
    setFormData((prev) => ({ ...prev, [name]: value }));
    
    // Auto calculate BMI
    if (name === 'weight' || name === 'height') {
      const weight = parseFloat(formData.weight) || 0;
      const height = parseFloat(formData.height) || 0;
      if (weight > 0 && height > 0) {
        const bmi = (weight / ((height / 100) ** 2)).toFixed(1);
        setFormData((prev) => ({ ...prev, bmi }));
      }
    }
  };

  const handleNext = () => {
    if (currentStep < steps.length - 1) {
      setCurrentStep(currentStep + 1);
    }
  };

  const handlePrev = () => {
    if (currentStep > 0) {
      setCurrentStep(currentStep - 1);
    }
  };

  const handleSubmit = async () => {
    setLoading(true);
    setError('');

    try {
      const response = await clinicalApi.submitCheckup(formData);
      navigate(`/result/${response.checkupId}`);
    } catch (err) {
      setError(err.message || 'Failed to submit checkup');
    } finally {
      setLoading(false);
    }
  };

  const renderStep = () => {
    switch (currentStep) {
      case 0:
        return (
          <MetricInputGroup
            title="Basic Health Metrics"
            metrics={basicMetrics}
            values={formData}
            onChange={handleInputChange}
          />
        );
      case 1:
        return (
          <MetricInputGroup
            title="Blood Test Results"
            metrics={bloodTestMetrics}
            values={formData}
            onChange={handleInputChange}
          />
        );
      case 2:
        return (
          <LifestyleSection
            values={formData}
            onChange={handleInputChange}
          />
        );
      case 3:
        return (
          <div className="space-y-4">
            <div className="bg-blue-50 p-6 rounded-lg">
              <h3 className="font-bold text-lg mb-4">Review Your Information</h3>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4 text-sm">
                {Object.entries(formData).map(([key, value]) => (
                  value && (
                    <div key={key}>
                      <p className="text-gray-600 font-medium">{key}</p>
                      <p className="text-gray-900">{value}</p>
                    </div>
                  )
                ))}
              </div>
            </div>
            <p className="text-sm text-gray-600">
              Click "Submit" to send your health data for AI analysis.
            </p>
          </div>
        );
      default:
        return null;
    }
  };

  return (
    <div className="max-w-2xl mx-auto">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-800 mb-2">Health Checkup Form</h1>
        <p className="text-gray-600">Please provide your health information for AI analysis</p>
      </div>

      <FormStepper
        steps={steps}
        currentStep={currentStep}
        onStepClick={(index) => setCurrentStep(index)}
      />

      {error && (
        <div className="mb-4 p-4 bg-red-100 text-red-700 rounded-lg">
          {error}
        </div>
      )}

      <div className="mb-8">
        {renderStep()}
      </div>

      <div className="flex justify-between gap-4">
        <Button
          variant="outline"
          onClick={handlePrev}
          disabled={currentStep === 0}
        >
          Previous
        </Button>

        {currentStep < steps.length - 1 ? (
          <Button onClick={handleNext}>Next</Button>
        ) : (
          <Button
            variant="success"
            onClick={handleSubmit}
            loading={loading}
          >
            Submit Checkup
          </Button>
        )}
      </div>
    </div>
  );
};

export default ClinicalForm;