/**
 * LifestyleSection Component - Nhập thông tin lối sống
 */
import Input from '../ui/Input';

export const LifestyleSection = ({
  values = {},
  onChange,
  errors = {},
}) => {
  const lifestyleMetrics = [
    {
      name: 'sleepHours',
      label: 'Sleep Hours per Day',
      placeholder: 'e.g., 7',
      type: 'number',
      step: '0.5',
    },
    {
      name: 'exerciseMinutes',
      label: 'Exercise Minutes per Week',
      placeholder: 'e.g., 150',
      type: 'number',
    },
    {
      name: 'stressLevel',
      label: 'Stress Level (1-10)',
      placeholder: 'e.g., 5',
      type: 'number',
      min: 1,
      max: 10,
    },
    {
      name: 'smokingStatus',
      label: 'Smoking Status',
      placeholder: 'e.g., Never/Former/Current',
      type: 'text',
    },
    {
      name: 'alcoholConsumption',
      label: 'Alcohol Consumption (units/week)',
      placeholder: 'e.g., 5',
      type: 'number',
    },
    {
      name: 'fruitsVegetablesServings',
      label: 'Fruits & Vegetables (servings/day)',
      placeholder: 'e.g., 5',
      type: 'number',
    },
  ];

  return (
    <div className="bg-white p-6 rounded-lg border border-gray-200">
      <h3 className="text-lg font-bold text-gray-800 mb-4">Lifestyle Information</h3>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {lifestyleMetrics.map((metric) => (
          <Input
            key={metric.name}
            label={metric.label}
            type={metric.type}
            placeholder={metric.placeholder}
            value={values[metric.name] || ''}
            onChange={(e) => onChange(metric.name, e.target.value)}
            error={errors[metric.name]}
            step={metric.step}
            min={metric.min}
            max={metric.max}
          />
        ))}
      </div>
    </div>
  );
};

export default LifestyleSection;
