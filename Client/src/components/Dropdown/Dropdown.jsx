import React, { useId } from 'react';
import './Dropdown.css';

const Dropdown = ({ label, options, onSelect, value }) => {
  const id = useId();
  const handleOptionClick = (e) => {
    onSelect(e.target.value);
  };

  return (
    <div className="dropdown">
      <label htmlFor={id}>{label}</label>
      <select id={id} onChange={handleOptionClick} value={value}>
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
    </div>
  );
};

export default Dropdown;
