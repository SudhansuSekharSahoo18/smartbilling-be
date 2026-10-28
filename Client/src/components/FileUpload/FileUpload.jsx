import React, { useState } from 'react';
import { Import } from '../../APIEndpoints.js'
import './FileUpload.css';

const FileUpload = (props) => {
    const [file, setFile] = useState(null);

    // Handles file selection
    const handleFileChange = (event) => {
        setFile(event.target.files[0]);
    };

    // Handles file upload on form submission
    const handleSubmit = async (event) => {
        
        // return;
        event.preventDefault();

        if (!file) {
            alert('Please select a file to upload.');
            return;
        }

        // Prepare form data
        const formData = new FormData();
        formData.append('file', file);

        try {
            // Send file using fetch
            const response = await fetch(props.ipAddress + 'api/' + Import, {
                method: 'POST',
                body: formData,
            });

            if (response.ok) {
                const result = await response.json();
                console.log('File uploaded successfully:', result);
            } else {
                console.error('File upload failed:', response.statusText);
            }
        } catch (error) {
            console.error('Error uploading file:', error);
        }
    };

    return (
        <form className='fileUpload' onSubmit={handleSubmit}>
            <label className='fileUpload-choose'>
                <input type="file" onChange={handleFileChange} />
                Choose file
            </label>
            <span className='fileUpload-name' title={file?.name}>{file ? file.name : 'No file chosen'}</span>
            <button type="submit" className='btn-secondary' disabled={!file}>Upload</button>
        </form>
    );
};

export default FileUpload;
