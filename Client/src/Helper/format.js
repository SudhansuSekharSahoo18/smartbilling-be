const rupees = new Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR', maximumFractionDigits: 2 });

// "₹1,499" / "₹1,499.50". Empty for missing values so grids don't show "₹NaN".
export const formatRupees = (value) =>
  value === null || value === undefined || value === '' || isNaN(value) ? '' : rupees.format(Number(value));
