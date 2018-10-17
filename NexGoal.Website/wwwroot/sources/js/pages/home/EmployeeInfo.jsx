import React from 'react';

export default function EmployeeInfo(props) {
    return (
        <div className="user-info">
            <img src={props.employee.avatar} alt="avatar" className="avatar" />
            <div className="nick-name">{props.employee.nickName}</div>
            <div className="user-name">{props.employee.fullName}</div>
            <div className="role role-PM">{props.employee.jobTitle}</div>
        </div>
    );
}