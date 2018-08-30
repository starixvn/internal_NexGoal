import React, { Component } from 'react';

class GroupFlowSetting extends Component {
    render() {
        let groupFlowSettings = this.props.groupFlowSettings;

        return (
            <table className="table table-bordered table-striped">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Team</th>
                        <th>Admin</th>
                        <th>Evaluator</th>
                        <th>Reviewer</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        groupFlowSettings.length === 0 &&
                        <tr>
                            <td className="empty-result" colSpan="5">No information available!</td>
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
                                                        {`${humanResource.fullName} (${humanResource.nickName})`}
                                                    </p>
                                                );
                                            })
                                        }
                                    </td>
                                    <td>
                                        {
                                            groupFlowSetting.evaluator ?
                                                `${groupFlowSetting.evaluator.fullName} (${groupFlowSetting.evaluator.nickName})`
                                                : ''
                                        }
                                    </td>
                                    <td>
                                        {
                                            groupFlowSetting.reviewer ?
                                                `${groupFlowSetting.reviewer.fullName} (${groupFlowSetting.reviewer.nickName})`
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