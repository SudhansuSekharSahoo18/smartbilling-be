import React from 'react';
import './Dropdown.css';

const Dropdown = ({ label, options, onSelect, value }) => {
  const handleOptionClick = (e) => {
    onSelect(e.target.value);
  };

  return (
    <div className="dropdown">
      <label>{label} </label>
      <select id="month" name="month" onChange={handleOptionClick} value={value}>
        {options.map((option) => (
          <option
            value={option.value}
            className="dropdown-item"
          >
            {option.label}
          </option>
        ))}
      </select>
    </div>
  );
};

export default Dropdown;