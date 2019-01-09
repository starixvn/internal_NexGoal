import React, { Component } from 'react';

import getResource from '../../../resourceHelper';

class EmployeeFlowSetting extends Component {
    render() {
        let employeeFlowSettings = this.props.employeeFlowSettings;

        return (
            <table className="table table-bordered table-striped">
                <thead>
                    <tr>
                        <th>{getResource('SymbolCount')}</th>
                        <th>{getResource('Code')}</th>
                        <th>{getResource('NickName')}</th>
                        <th>{getResource('FullName')}</th>
                        <th>{getResource('Team')}</th>
                        <th>{getResource('Evaluator')}</th>
                        <th>{getResource('Reviewer')}</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        employeeFlowSettings.length === 0 &&
                        <tr>
                            <td className="empty-result" colSpan="7">{getResource('NoRecordFound')}</td>
                        </tr>
                    }
                    {
                        employeeFlowSettings.length > 0 &&
                        employeeFlowSettings.map((employeeFlowSetting, index) => {
                            return (
                                <tr key={`employeeFlowSetting${employeeFlowSetting.employeeId}`}>
                                    <td className="text-center">{index + 1}</td>
                                    <td className="text-center">{employeeFlowSetting.code}</td>
                                    <td className="text-center">{employeeFlowSetting.nickName}</td>
                                    <td>{employeeFlowSetting.fullName}</td>
                                    <td>{employeeFlowSetting.groupName}</td>
                                    <td>
                                        {
                                            employeeFlowSetting.evaluator ?
                                                `${employeeFlowSetting.evaluator.nickName} (${employeeFlowSetting.evaluator.fullName})`
                                                : ''
                                        }
                                    </td>
                                    <td>
                                        {
                                            employeeFlowSetting.reviewer ?
                                                `${employeeFlowSetting.reviewer.nickName} (${employeeFlowSetting.reviewer.fullName})`
                                                : ''
                                        }</td>
                                </tr>
                            );
                        })
                    }
                </tbody>
            </table>
        );
    }
}

export default EmployeeFlowSetting;