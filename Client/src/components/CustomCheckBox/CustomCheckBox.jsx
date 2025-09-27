import React from 'react';
import './CustomCheckBox.css';

const CustomCheckBox = ({ label, isChecked, setIsChecked }) => {
    // const [isChecked, setIsChecked] = useState(false);

//   const handleCheckboxChange = (event) => {
//     setIsChecked(event.target.checked);
//   };

  return (
    <div className='CustomCheckBox'>
      <label>
        <input
          type="checkbox"
          checked={isChecked}
          onChange={(e) => setIsChecked(e.target.checked)}
        />
        {label}
      </label>
      {/* <p>{isChecked ? 'The checkbox is checked.' : 'The checkbox is not checked.'}</p> */}
    </div>
  );
};

export default CustomCheckBox;