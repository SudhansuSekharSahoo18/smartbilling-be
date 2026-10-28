import React from 'react';
import './CustomInput.css';

const CustomInput = ({ label, text, type, setText, isReadyOnly, placeholder }) => {
    return (
        <div className='customInput'>
            <label htmlFor={label}>{label}</label>
            <input id={label} type={type} value={text ?? ''} readOnly={isReadyOnly} placeholder={placeholder}
                onChange={(e) => setText(type === 'number' ? parseInt(e.target.value) : e.target.value)} />
        </div>
    );
};

export default CustomInput;
