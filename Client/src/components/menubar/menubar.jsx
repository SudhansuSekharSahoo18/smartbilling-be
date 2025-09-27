import React from 'react';
import './MenuBar.css';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Home from '../../Home/Home';
import Billing from '../../Home/Billing';
import Contact from '../../Home/Contact';

const MenuBar = () => {
  return (
    <Router>
          <div className="menu-bar">
              <nav>
                  <ul>
                      <li>
                          <Link to="/">Stock</Link>
                      </li>
                      <li>
                          <Link to="/pos">Pos</Link>
                      </li>
                      <li>
                          <Link to="/bills">Bills</Link>
                      </li>
                      <li>
                          <Link to="/generateBarcode">Barcode</Link>
                      </li>
                  </ul>
              </nav>

              <Routes>
                  <Route path="/" element={<Home />} />
                  <Route path="/pos" element={<Billing />} />
                  <Route path="/bills" element={<Contact />} />
                  <Route path="/generateBarcode" element={<Contact />} />
              </Routes>
          </div>
      </Router>
    // <div className="menu-bar">
    //   <ul>
    //     <li><a href="#home">Home</a></li>
    //     <li><a href="#about">About</a></li>
    //     <li><a href="#services">Services</a></li>
    //     <li><a href="#contact">Contact</a></li>
    //   </ul>
    // </div>
  );
};

export default MenuBar;
