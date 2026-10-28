import React, { useRef, useState } from 'react';
import { readError } from '../../Helper/apiHelper.js';
import './FileUpload.css';

// Posts the chosen file (form field "file") to `url`, then calls onUploaded with the JSON reply.
const FileUpload = ({ url, accept, buttonText = 'Upload', title, onUploaded, onError }) => {
    const [file, setFile] = useState(null);
    const [isUploading, setIsUploading] = useState(false);
    const inputRef = useRef(null);

    // Handles file selection
    const handleFileChange = (event) => {
        setFile(event.target.files[0] ?? null);
    };

    // Handles file upload on form submission
    const handleSubmit = async (event) => {
        event.preventDefault();

        if (!file || !url) {
            return;
        }

        // Prepare form data
        const formData = new FormData();
        formData.append('file', file);

        setIsUploading(true);
        try {
            const response = await fetch(url, {
                method: 'POST',
                body: formData,
            });

            if (response.ok) {
                setFile(null);
                if (inputRef.current) inputRef.current.value = '';
                onUploaded?.(await response.json());
            } else {
                onError?.(await readError(response));
            }
        } catch (error) {
            onError?.(`Upload failed: ${error.message}`);
        } finally {
            setIsUploading(false);
        }
    };

    return (
        <form className='fileUpload' onSubmit={handleSubmit} title={title}>
            <label className='fileUpload-choose'>
                <input ref={inputRef} type="file" accept={accept} onChange={handleFileChange} />
                Choose file
            </label>
            <span className='fileUpload-name' title={file?.name}>{file ? file.name : 'No file chosen'}</span>
            <button type="submit" className='btn-secondary' disabled={!file || !url || isUploading}>
                {isUploading ? 'Importing…' : buttonText}
            </button>
        </form>
    );
};

export default FileUpload;
