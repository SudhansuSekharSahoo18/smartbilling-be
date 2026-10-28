import React from 'react';

const ErrorView = ({ message }) => {

  return (
    <div className='card empty-state'>
      <h2 className='card-title'>Something went wrong</h2>
      <p className='muted'>{message}</p>
    </div>
  );
};

export default ErrorView;
