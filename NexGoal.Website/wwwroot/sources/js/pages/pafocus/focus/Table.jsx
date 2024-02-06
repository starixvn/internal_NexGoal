import React, { Component, Fragment } from 'react';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';
import moment from 'moment';

import getResource from '../../../resourceHelper';
import { DateFormat } from '../../../constants';
import Row from './Row';
import StickyBlock from '../../../shared/components/StickyBlock';

export default class Table extends Component {
    shouldComponentUpdate(nextProps) {
        return this.props.data !== nextProps.data
            || this.props.period !== nextProps.period
            || this.props.focusEvaluationLevelOptions !== nextProps.focusEvaluationLevelOptions;
    }

    getTooltipBestSuitedToTeamGoals = () => {
        return (
            <Tooltip id="tooltip-focus-note" className="focus-note">
                Best Suited to Team Goals:
                <span> I would always want him or her on my team (ability to work well with others).</span>
            </Tooltip>
        );
    };

    getTooltipDeserveHighestReward = () => {
        return (
            <Tooltip id="tooltip-focus-note" className="focus-note">
                Deserve Highest Reward (Increment And Bonus):
                <span> I would want to award this person the highest possible benefit (increase and bonus) which is available for her/his job grade.</span>
            </Tooltip>
        );
    };

    renderHeaderTableFocus = () => {
        return (
            <Fragment>
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
                    <th className="width-90">{getResource('PAFinal')}</th>
                    <th className="objective" colSpan="2">{getResource('AccomplishedCurrentObjectives')}</th>
                    <th className="objective" colSpan="2">{getResource('NextObjectives')}</th>
                </tr>
            </Fragment>
        );
    };

    handleExportToExcelTable = event => {
        event.preventDefault();
        this.props.onExportToExcel();
    };

    render() {
        const period = this.props.period,
            focusStartDate = period && period.focusStartDate ? moment(period.focusStartDate).format(DateFormat) : '',
            focusEndDate = period && period.focusEndDate ? moment(period.focusEndDate).format(DateFormat) : '';
        const pafocuses = this.props.data;

        return (
            <Fragment>
                {
                    focusStartDate && focusEndDate &&
                    <div className="title">
                        <div className="pull-left">
                            Duration: {`${focusStartDate} - ${focusEndDate}`}
                        </div>
                        <div className="pull-right">
                            <span class="icon-save-black-18dp" onClick={this.props.handleSubmit} />
                            <span class="icon-file-excel" onClick={this.handleExportToExcelTable} />
                        </div>
                    </div>
                }
                <StickyBlock threshold={236}>
                    <table className="table table-bordered table-striped pa-focus-table">
                        <thead>
                            {
                                this.renderHeaderTableFocus()
                            }
                        </thead>
                    </table>
                </StickyBlock>
                <table className="table table-bordered table-striped pa-focus-table">
                    <thead>
                        {
                            this.renderHeaderTableFocus()
                        }
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
                                                        disciplineStartDate={period && period.disciplineStartDate}
                                                        disciplineEndDate={period && period.disciplineEndDate}
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
                    <tfoot>
                        {
                            this.props.isDisplayButton &&
                            <tr className="action-section button-block text-center">
                                <td colSpan="12">
                                    <button className="btn btn-cancel" onClick={this.props.handleFilterSubmit}>{getResource('Reset')}</button>
                                    <button className="btn btn-primary" onClick={this.props.handleSubmit}>{getResource('Save')}</button>
                                </td>
                            </tr>
                        }
                    </tfoot>
                </table>
            </Fragment>
        );
    }
}