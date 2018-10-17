import React, { PureComponent } from 'react';

export default class EmployeeInfo extends PureComponent {
    render() {
        let nickName = document.getElementById('employeeInfo-nickName').innerText;
        let fullName = document.getElementById('employeeInfo-fullName').innerText;
        let jobTitle = document.getElementById('employeeInfo-jobTitle').innerText;
        let avatar = document.getElementById('employeeInfo-avatar').getAttribute('src');

        return (
            <div className="user-info">
                <img src={avatar} alt="avatar" className="avatar" />
                <div className="nick-name">{nickName}</div>
                <div className="user-name">{fullName}</div>
                <div className="role role-PM">{jobTitle}</div>
            </div>
        );
    }
}