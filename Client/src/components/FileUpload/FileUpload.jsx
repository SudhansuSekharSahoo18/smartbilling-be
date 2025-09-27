import React, { useState } from 'react';
import { Import } from '../../APIEndpoints.js'

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
        <form onSubmit={handleSubmit}>
            <input type="file" onChange={handleFileChange} />
            <button type="submit">Upload File</button>
        </form>
    );
};

export default FileUpload;
