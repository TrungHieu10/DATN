/**
 * FormStepper Component - Multi-step form navigation
 */
export const FormStepper = ({ steps = [], currentStep = 0, onStepClick }) => {
  return (
    <div className="flex items-center justify-between mb-8">
      {steps.map((step, index) => (
        <div key={index} className="flex-1">
          {/* Step Circle */}
          <div className="flex items-center">
            <button
              onClick={() => onStepClick && onStepClick(index)}
              disabled={index > currentStep}
              className={`w-10 h-10 rounded-full font-bold transition-all ${
                index <= currentStep
                  ? 'bg-blue-600 text-white cursor-pointer'
                  : 'bg-gray-300 text-gray-600 cursor-not-allowed'
              }`}
            >
              {index + 1}
            </button>

            {/* Step Label */}
            <div className="ml-2">
              <p className="text-sm font-medium text-gray-900">{step}</p>
            </div>

            {/* Connector Line */}
            {index < steps.length - 1 && (
              <div
                className={`flex-1 h-1 mx-2 transition-all ${
                  index < currentStep ? 'bg-blue-600' : 'bg-gray-300'
                }`}
              ></div>
            )}
          </div>
        </div>
      ))}
    </div>
  );
};

export default FormStepper;
