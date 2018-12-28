import React, { Component } from 'react';

import getResource from '../../../resourceHelper';

class GroupFlowSetting extends Component {
    render() {
        let groupFlowSettings = this.props.groupFlowSettings;

        return (
            <table className="table table-bordered table-striped">
                <thead>
                    <tr>
                        <th>{getResource('SymbolCount')}</th>
                        <th>{getResource('Team')}</th>
                        <th>{getResource('Admin')}</th>
                        <th>{getResource('Evaluator')}</th>
                        <th>{getResource('Reviewer')}</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        groupFlowSettings.length === 0 &&
                        <tr>
                            <td className="empty-result" colSpan="5">{getResource('NoRecordFound')}</td>
                        </tr>
                    }
                    {
                        groupFlowSettings.length > 0 &&
                        groupFlowSettings.map((groupFlowSetting, index) => {
                            return (
                                <tr key={`groupFlowSetting${groupFlowSetting.groupId}`}>
                                    <td className="text-center">{index + 1}</td>
                                    <td>{groupFlowSetting.name}</td>
                                    <td>
                                        {
                                            groupFlowSetting.humanResources.length > 0 &&
                                            groupFlowSetting.humanResources.map(humanResource => {
                                                return (
                                                    <p key={`humanResource${humanResource.evaluatorId}`}>
                                                        {`${humanResource.nickName} (${humanResource.fullName})`}
                                                    </p>
                                                );
                                            })
                                        }
                                    </td>
                                    <td>
                                        {
                                            groupFlowSetting.evaluator ?
                                                `${groupFlowSetting.evaluator.nickName} (${groupFlowSetting.evaluator.fullName})`
                                                : ''
                                        }
                                    </td>
                                    <td>
                                        {
                                            groupFlowSetting.reviewer ?
                                                `${groupFlowSetting.reviewer.nickName} (${groupFlowSetting.reviewer.fullName})`
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

export default GroupFlowSetting;