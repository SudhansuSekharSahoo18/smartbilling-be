import React, { useRef, useEffect, useState } from 'react';

const ErrorView = ({ exception }) => {

  return (
    <div >
     {exception.message}
    </div>
  );
};

export default ErrorView;