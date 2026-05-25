/**
 * MetricInputGroup Component - Input nhóm chỉ số khám
 */
import Input from '../ui/Input';

export const MetricInputGroup = ({
  title,
  metrics = [],
  values = {},
  onChange,
  errors = {},
}) => {
  return (
    <div className="bg-white p-6 rounded-lg border border-gray-200">
      <h3 className="text-lg font-bold text-gray-800 mb-4">{title}</h3>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {metrics.map((metric) => (
          <Input
            key={metric.name}
            label={metric.label}
            type="number"
            placeholder={metric.placeholder}
            value={values[metric.name] || ''}
            onChange={(e) => onChange(metric.name, e.target.value)}
            error={errors[metric.name]}
            required={metric.required}
            step={metric.step || '0.1'}
          />
        ))}
      </div>
    </div>
  );
};

export default MetricInputGroup;
