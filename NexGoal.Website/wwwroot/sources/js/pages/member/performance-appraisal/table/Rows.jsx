import React, { Component, Fragment } from 'react';
import { PerformanceProcess } from '../../../../constants';

class Row extends Component {
    constructor(props) {
        super(props);
    }

    shouldComponentUpdate(nextProps) {
        if (this.props.groupName !== nextProps.groupName
            || JSON.stringify(this.props.performanceAppraisals) !== JSON.stringify(nextProps.performanceAppraisals)) {
            return true;
        }

        return false;
    }

    renderPAByProcess = (paProcesses, processId) => {
        let processes = paProcesses.filter(process => process.processId === processId);
        let grade = processes.map(process => {
            return process.performanceRecord ? process.performanceRecord.grade.name : '';
        }).join(', ');
        return <span>{grade}</span>;
    };

    handleClick = (paRecordId) => {
        this.props.onClickEditPA(paRecordId);
    }

    render() {
        const performanceRecords = this.props.data.performanceAppraisals;
        return (
            <Fragment>
                <tr>
                    <td colSpan={this.props.colSpan}>{this.props.data.groupName}</td>
                </tr>
                {
                    performanceRecords &&
                    performanceRecords.map(record => {
                        return (
                            <tr>
                                <td>{record.employee.nickName}</td>
                                <td>{record.toeic}</td>
                                <td>{this.renderPAByProcess(record.processes, PerformanceProcess.Self)}</td>
                                <td>{this.renderPAByProcess(record.processes, PerformanceProcess.ProjectManager)}</td>
                                <td>{this.renderPAByProcess(record.processes, PerformanceProcess.Evaluator)}</td>
                                <td>{this.renderPAByProcess(record.processes, PerformanceProcess.Reviewer)}</td>
                                <td>
                                    <a className="fa fa-pencil-square-o" title="Edit"
                                        onClick={() => this.handleClick(record.paRecordId)} />
                                </td>
                            </tr>
                        );
                    })
                }
            </Fragment>
        );
    }
}

export default Row;