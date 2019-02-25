import React, { Component } from 'react';
import { Tabs, Tab } from 'react-bootstrap';
import { PerformanceProcess } from '../../../constants';
import getResource from '../../../resourceHelper';
import PerformanceRecord from '../PerformanceRecord';

export default class PAProcessTab extends Component {
    constructor(props) {
        super(props);

        this.state = {
            activeTabKey: 1
        };
    }

    renderRecordByProcess = (processId) => {
        let paProcesses = this.props.processes.filter(p => p.performanceRecord && p.processId === processId);
        if (paProcesses.length > 0) {
            return paProcesses.map((process, index) => {
                process.performanceRecord.employees = processId === PerformanceProcess.Self ? null : this.props.employees;
                process.performanceRecord.isHost = false;
                return (
                    <PerformanceRecord key={index} data={process.performanceRecord} isHideProjectTag />
                );
            });
        }

        return <span>{getResource('NoRecordFound')}</span>;
    };

    handleSelectTab = (key) => {
        this.setState({ activeTabKey: key });
    }

    render() {
        const selfRecord = this.renderRecordByProcess(PerformanceProcess.Self);
        const pmRecords = this.renderRecordByProcess(PerformanceProcess.ProjectManager);
        const evaluatorRecord = this.renderRecordByProcess(PerformanceProcess.Evaluator);
        const reviewerRecord = this.renderRecordByProcess(PerformanceProcess.Reviewer);

        return (
            <div>
                <Tabs id="tabs" activeKey={this.state.activeTabKey} onSelect={this.handleSelectTab} animation={false}>
                    <Tab eventKey={1} title={getResource("Self")}>
                        {selfRecord}
                    </Tab>
                    <Tab eventKey={2} title={getResource("ProjectManager")}>
                        {pmRecords}
                    </Tab>
                    <Tab eventKey={3} title={getResource("Evaluator")}>
                        {evaluatorRecord}
                    </Tab>
                    <Tab eventKey={4} title={getResource("Reviewer")}>
                        {reviewerRecord}
                    </Tab>
                </Tabs>
            </div>
        );
    }
}