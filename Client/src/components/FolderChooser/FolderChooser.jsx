// src/FolderChooser.js

import React, { useState } from 'react';

const FolderChooser = () => {
    const [folderHandle, setFolderHandle] = useState(null);
    const [fileList, setFileList] = useState([]);
    const [selectedDirectory, setSelectedDirectory] = useState(null);

    const handleFolderSelect = async () => {
        try {
            const handle = await window.showDirectoryPicker();
            setFolderHandle(handle);
            console.log(handle)
            // const files = [];
            // for await (const entry of handle.values()) {
            //     if (entry.kind === 'file') {
            //         files.push(entry.name);
            //     }
            // }
            // setFileList(files);
        } catch (error) {
            console.error('Error selecting folder:', error);
        }
    };

    return (
        <div>
            <div></div>
            <button onClick={handleFolderSelect}>Browse</button>
            {folderHandle && (
                <div>
                    <h3>Selected Folder: {folderHandle.name}</h3>
                    <ul>
                        {fileList.map((fileName, index) => (
                            <li key={index}>{fileName}</li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
};

export default FolderChooser;
