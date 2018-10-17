import React from 'react';


export default function EmployeeInfo(props) {
    if (props.employee) {
        return (
            <div className="user-info">
                <img src={`data:image/png;base64 ${props.employee.base64Image}`} alt="avatar" className="avatar" />
                <div className="nick-name">{props.employee.nickName}</div>
                <div className="user-name">{props.employee.fullName}</div>
                <div className="role role-PM">{props.employee.jobTitle}</div>
            </div>
        );
    }

    return '';
}