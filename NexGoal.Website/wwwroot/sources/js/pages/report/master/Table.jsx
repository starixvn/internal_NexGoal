import React, { Component, Fragment } from 'react';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';

import getResource from '../../../resourceHelper';
import { NominationStatuses } from '../../../constants';
import { getNominationClassName, noDataIndication } from '../../../common-functions';

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
        let header = this.refs.stickyHeader;

        window.addEventListener('scroll', function (e) {
            if (window.scrollY > 236) {
                header.classList.add('sticky');
            } else {
                header.classList.remove('sticky');
            }
        });
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

    handleObjectiveMessage = record => {
        this.props.onViewObjectiveMessage(record);
    }

    handleViewPADetail = paRecordId => this.props.handleViewPADetail(paRecordId)

    renderPAAdjustmentIndicator = (gradeId, originalGradeId) => {
        let gradeChangeClass = '';
        if (parseInt(gradeId) > parseInt(originalGradeId)) {
            gradeChangeClass = 'icon-increase-arrow';
        }

        if (parseInt(gradeId) < parseInt(originalGradeId)) {
            gradeChangeClass = 'icon-descrease-arrow';
        }
        return <span className={gradeChangeClass} />;
    }

    renderRecord = (record, index) => {
        let rowSpan = record.awards.length === 0 ? 1 : record.awards.length;
        return (
            <Fragment key={record.employeeId}>
                <tr className={this.getBackgroundClass(record)}>
                    <td rowSpan={rowSpan} className="text-center">{index + 1}</td>
                    <td rowSpan={rowSpan} >{record.nickName} ({record.fullName})</td>
                    <td rowSpan={rowSpan} >{record.jobTitle}</td>
                    <td rowSpan={rowSpan} className="text-center">
                        <a onClick={this.handleViewPADetail.bind(this, record.paRecordId)}>{record.grade}</a>
                        {this.renderPAAdjustmentIndicator(record.gradeId, record.originalGradeId)}
                    </td>
                    <td rowSpan={rowSpan} className="text-center">{record.contribution}</td>
                    <td rowSpan={rowSpan} className="text-center">{record.bonusGiven}</td>
                    <td rowSpan={rowSpan} className="text-center">{record.isReadyPromotion === 'true' ? 'Yes' : 'No'}</td>
                    <td rowSpan={rowSpan} className="text-center">
                        {(record.nextObjective || record.currentObjective) && <a className="fa fa-search" onClick={this.handleObjectiveMessage.bind(this, record)}></a>}
                    </td>
                    <td>{record.awards[0] && record.awards[0].title}</td>
                    <td className="text-center">{record.awards[0] &&
                        <span className={getNominationClassName(record.awards[0].statusId)}>
                            {record.awards[0].status}
                        </span>}
                    </td>
                </tr>

                {record.awards.slice(1).map((a, i) =>
                    <tr key={i}>
                        <td>{a.title}</td>
                        <td>
                            <span className={getNominationClassName(a.statusId)}>
                                {a.status}
                            </span>
                        </td>
                    </tr>)
                }
            </Fragment>
        );
    }
    renderData = (data, index) =>
        <Fragment key={index}>
            <tr className="grou-row">
                <td colSpan={12} className="title"> {data.group}</td>
            </tr>
            {data.records.map(this.renderRecord)}
        </Fragment>

    getBackgroundClass = (record) => {
        let className = '';
        let contribution = record.contributionId;
        let bonusGiven = record.bonusGivenId;
        let isReadyPromotion = record.isReadyPromotion ? record.isReadyPromotion === 'true' : false;

        if (contribution + bonusGiven > 8) {
            className = `noticeable-performance ${isReadyPromotion ? 'outstanding' : 'very-good'}`;
        } else if (contribution > 0 && bonusGiven > 0 && contribution + bonusGiven < 4) {
            className = 'noticeable-performance poor';
        }

        return className;
    }

    render() {
        let content = this.props.tableData.length > 0 ? this.props.tableData.map(this.renderData)
            : <tr><td className="text-center" colSpan={11}>{noDataIndication()}</td></tr>;

        return (
            <table className="table table-bordered table-striped tbl-master-report">
                <thead ref='stickyHeader'>
                    <tr>
                        <th rowSpan={2} className="order">{getResource('SymbolCount')}</th>
                        <th rowSpan={2} className="">{getResource('Employee')}</th>
                        <th rowSpan={2} className="">{getResource('JobTitle')}</th>
                        <th rowSpan={2} className="">{getResource('PAFinal')}</th>
                        <th rowSpan={2} className="best suited">
                            {getResource('BestSuitedToTeamGoals')}
                            <OverlayTrigger placement="top" overlay={this.getTooltipBestSuitedToTeamGoals()}>
                                <i className="fa fa-info-circle" />
                            </OverlayTrigger>
                        </th>
                        <th rowSpan={2} className="deserve highest">
                            Deserve Highest Reward (Increment And Bonus)
                            <OverlayTrigger placement="top" overlay={this.getTooltipDeserveHighestReward()}>
                                <i className="fa fa-info-circle" />
                            </OverlayTrigger>
                        </th>
                        <th rowSpan={2} className="">{getResource('ReadyForPromotion')}</th>
                        <th rowSpan={2} className=""> {getResource('Objectives')}</th>
                        <th colSpan={2}>{getResource('Nomination')}</th>
                    </tr>
                    <tr>
                        <th className="nomination-period">{getResource('Period')}</th>
                        <th className="nomination-status">{getResource('Status')}</th>
                    </tr>
                </thead>
                <tbody>
                    {content}
                </tbody>
            </table>
        );
    }
}