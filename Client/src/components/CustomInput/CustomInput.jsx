import React from 'react';
import './CustomInput.css';

const CustomInput = ({ label, text, type, setText, isReadyOnly }) => {
    return (
        <div className='customInput'>
            <label>{label} </label>
            <input id={label} type={type} value={text} readOnly={isReadyOnly}
                onChange={(e) => setText(type === 'number' ? parseInt(e.target.value) : e.target.value)} />
        </div>
    );
};

export default CustomInput;