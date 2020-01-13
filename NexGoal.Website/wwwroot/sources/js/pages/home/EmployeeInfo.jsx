import React, { PureComponent } from 'react';

export default class EmployeeInfo extends PureComponent {
    render() {
        const employee = this.props.employee;
        return (
            <div className="user-info box">
                <img src={employee.avatar} alt="avatar" className="avatar" />
                <div>
                    <div className="nick-name role-member">{employee.nickName}</div>
                    <div className="user-name">{employee.fullName}</div>
                    <div className="role role-PM">{employee.jobTitle}</div>
                </div>
            </div>
        );
    }
}