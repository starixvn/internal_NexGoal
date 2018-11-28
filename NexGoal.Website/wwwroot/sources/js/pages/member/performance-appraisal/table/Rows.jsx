import React, { Component, Fragment } from 'react';
import { PerformanceProcess, PAStatus } from '../../../../constants';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';
import getResource from '../../../../resourceHelper';

class Rows extends Component {
    constructor(props) {
        super(props);
    }

    shouldComponentUpdate(nextProps) {
        if (JSON.stringify(this.props.data) !== JSON.stringify(nextProps.data)) {
            return true;
        }

        return false;
    }

    renderToeic = (toeicPolicy, toeic) => {
        let toeicClass = '';
        if (toeicPolicy !== null && toeicPolicy > 0) toeicClass = 'role-member';
        if (toeicPolicy === null || toeicPolicy < 0) toeicClass = 'red';
        return <span className={`bold ${toeicClass}`}>{toeic ? toeic : getResource('N/A')}</span>;
    }

    renderPAForReviewer = (record) => {
        let reviewerProcess = record.processes.filter(process => process.processId === PerformanceProcess.Reviewer);
        let paGrade;
        let gradeChangeClass = '';
        if (record.isEdited) {
            paGrade = record.grade.name;
        } else {
            paGrade = record.originalGrade.name;
        }
        if (parseInt(record.gradeId) > parseInt(record.originalGradeId)) gradeChangeClass = 'icon-increase-arrow';
        if (parseInt(record.gradeId) < parseInt(record.originalGradeId)) gradeChangeClass = 'icon-descrease-arrow';
        let displayPAGrade = record.statusId === PAStatus.Closed ?
            <span>{paGrade}</span> :
            <a onClick={() => this.handleClick(record.paRecordId)}>{paGrade}</a>;
        return (
            <Fragment>
                {
                    reviewerProcess.length > 0 &&
                    <span className="icon-note" />
                }
                {displayPAGrade}
                <span className={gradeChangeClass} />
            </Fragment>
        );
    };

    renderPAByProcess = (paProcesses, processId) => {
        let processes = paProcesses.filter(process => process.processId === processId);
        let grade = processes.map(process => {
            return process.performanceRecord ? process.performanceRecord.grade.name : '';
        }).join(', ');
        let tooltip;

        if (processId === PerformanceProcess.ProjectManager) {
            tooltip = (
                <Tooltip>
                    {
                        processes.map(process => {
                            return (
                                <p>
                                    {`${process.evaluator.nickName}: ${process.performanceRecord
                                        ? process.performanceRecord.grade.name : "N/A"} `}
                                </p>
                            );
                        })
                    }
                </Tooltip>
            );

            return (
                <OverlayTrigger placement="bottom" overlay={tooltip}>
                    <span className="pointer">{grade}</span>
                </OverlayTrigger>
            );
        } else {
            return <span>{grade}</span>;
        }
    }

    renderStatus = (isEdited, paRecordId) => {
        if (isEdited) {
            return (<div>
                <input type="checkbox" className="checkbox-custom" id={`chb-${paRecordId}`} checked />
                <label htmlFor={`chb-${paRecordId}`} />
            </div>);
        } else {
            return (<div>
                <input type="checkbox" className="checkbox-custom"
                    id={`chb-${paRecordId}`}
                    onClick={() => this.props.onQuickEditPA(paRecordId)} />
                <label htmlFor={`chb-${paRecordId}`} />
            </div>);
        }
    }

    renderNickName = (record) => {
        let displayNickname = <span>{record.employee.nickName}</span>;
        if (record.statusId !== PAStatus.InProgress) {
            displayNickname = <a onClick={() => this.handleClick(record.paRecordId)}>{record.employee.nickName}</a>;
        }
        return displayNickname;
    }
    handleClick = (paRecordId) => {
        this.props.onClickEditPA(paRecordId);
    }

    render() {
        const performanceRecords = this.props.data.performanceAppraisals;
        return (
            <Fragment>
                <tr className="group-row">
                    <td colSpan={this.props.colSpan}>{this.props.data.groupName}</td>
                </tr>
                {
                    performanceRecords &&
                    performanceRecords.map(record => {
                        return (
                            <tr>
                                <td>{this.renderNickName(record)}</td>
                                <td className="text-center">{this.renderToeic(record.toeicPolicy, record.toeic)}</td>
                                <td className="text-center">{this.renderPAByProcess(record.processes, PerformanceProcess.Self)}</td>
                                <td className="text-center">
                                    {this.renderPAByProcess(record.processes, PerformanceProcess.ProjectManager)}</td>
                                <td className="text-center">{this.renderPAByProcess(record.processes, PerformanceProcess.Evaluator)}</td>
                                <td className="text-center">{this.renderPAForReviewer(record)}</td>
                                <td className="text-center">{this.renderStatus(record.isEdited, record.paRecordId)}</td>
                            </tr>
                        );
                    })
                }
            </Fragment>
        );
    }
}

export default Rows;