import React from 'react';
import { Link } from 'react-router-dom';

const ErrorPageNotFoundView = () => {

  return (
    <div className='card empty-state'>
      <h2 className='card-title'>Page not found</h2>
      <p className='muted'>This page doesn't exist. <Link to="/">Go to the dashboard</Link>.</p>
    </div>
  );
};

export default ErrorPageNotFoundView;
