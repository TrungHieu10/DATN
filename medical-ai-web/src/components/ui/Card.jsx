/**
 * Card Component - Container tái sử dụng
 */
export const Card = ({
  children,
  className = '',
  header,
  footer,
  hover = false,
  shadow = 'md',
}) => {
  const shadowClasses = {
    sm: 'shadow-sm',
    md: 'shadow-md',
    lg: 'shadow-lg',
    xl: 'shadow-xl',
  };

  return (
    <div
      className={`bg-white rounded-lg ${shadowClasses[shadow]} overflow-hidden ${
        hover ? 'hover:shadow-lg transition-shadow duration-200 cursor-pointer' : ''
      } ${className}`}
    >
      {header && <div className="px-6 py-4 border-b border-gray-200">{header}</div>}
      <div className="px-6 py-4">{children}</div>
      {footer && <div className="px-6 py-4 border-t border-gray-200 bg-gray-50">{footer}</div>}
    </div>
  );
};

export default Card;
