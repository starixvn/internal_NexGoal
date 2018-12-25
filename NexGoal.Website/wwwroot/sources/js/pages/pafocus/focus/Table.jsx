import React, { Component, Fragment } from 'react';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';

import getResource from '../../../resourceHelper';
import Row from './Row';

export default class Table extends Component {
    shouldComponentUpdate(nextProps) {
        if (this.props.data !== nextProps.data
            || this.props.focusEvaluationLevelOptions !== nextProps.focusEvaluationLevelOptions) {
            return true;
        }

        return false;
    }

    getTooltipBestSuitedToTeamGoals = () => {
        return (
            <Tooltip id="tooltip-focus-note" className="focus-note">
                Best Suited to Team Goals:
                <span> I would always want him or her on my team (ability to work well with others).</span>
            </Tooltip>
        );
    }

    getTooltipDeserveHighestReward = () => {
        return (
            <Tooltip id="tooltip-focus-note" className="focus-note">
                Deserve Highest Reward (Increment And Bonus):
                <span> I would want to award this person the highest possible benefit (increase and bonus) which is available for her/his job grade.</span>
            </Tooltip>
        );
    }

    render() {
        const pafocuses = this.props.data;

        return (
            <table className="table table-bordered table-striped pa-focus-table">
                <thead>
                    <tr className="title">
                        <td colspan="12">Duration: 01 Nov 2018 - 17 Nov 2018</td>
                    </tr>
                    <tr>
                        <th className="width-40">{getResource('SymbolCount')}</th>
                        <th className="width-90">{getResource('NickName')}</th>
                        <th className="width-200">{getResource('FullName')}</th>
                        <th className="width-300">{getResource('JobTitle')}</th>
                        <th className="width-200">
                            Best Suited to Team Goals
                            <OverlayTrigger placement="top" overlay={this.getTooltipBestSuitedToTeamGoals()}>
                                <i className="fa fa-info-circle" />
                            </OverlayTrigger>
                        </th>
                        <th className="width-200">
                            Deserve Highest Reward (Increment And Bonus)
                            <OverlayTrigger placement="top" overlay={this.getTooltipDeserveHighestReward()}>
                                <i className="fa fa-info-circle" />
                            </OverlayTrigger>
                        </th>
                        <th className="width-150">{getResource('ReadyForPromotion')}</th>
                        <th className="width-90">PA Grade</th>
                        <th className="objective" colSpan="2">{getResource('AccomplishedCurrentObjectives')}</th>
                        <th className="objective" colSpan="2">{getResource('NextObjectives')}</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        pafocuses.length === 0 &&
                        <tr>
                            <td className="empty-result" colSpan="13">{getResource('NoRecordFound')}</td>
                        </tr>
                    }
                    {
                        pafocuses.length > 0 &&
                        pafocuses.map(record => {
                            return (
                                <Fragment key={`record${record.groupCode}`}>
                                    <tr className="group-row">
                                        <td colSpan="13">{record.groupName}</td>
                                    </tr>
                                    {
                                        record.members.map((member, index) => {
                                            return (
                                                <Row key={`member${member.paFocusId}`}
                                                    data={member}
                                                    rowIndex={index + 1}
                                                    focusEvaluationLevelOptions={this.props.focusEvaluationLevelOptions}
                                                    onChange={this.props.onChange}
                                                />
                                            );
                                        })
                                    }
                                </Fragment>
                            );
                        })
                    }
                </tbody>
            </table>
        );
    }
}