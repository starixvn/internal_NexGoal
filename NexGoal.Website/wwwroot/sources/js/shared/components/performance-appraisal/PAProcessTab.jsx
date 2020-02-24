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

    renderRecordByProcess = processId => {
        const processes = this.props.processes.filter(p => p.processId === processId && p.relatedPerformanceRecords.length > 0);
        if (processes.length > 0) {
            return processes.map(process => process.relatedPerformanceRecords
                .map(record => <PerformanceRecord key={record.performanceRecordId} data={record} />)
            );
        }

        return <span>{getResource('NoRecordFound')}</span>;
    };

    handleSelectTab = key => {
        this.setState({ activeTabKey: key });
    }

    render() {
        return (
            <div>
                <Tabs id="tabs" activeKey={this.state.activeTabKey} onSelect={this.handleSelectTab} animation={false}>
                    <Tab eventKey={1} title={getResource('Self')}>
                        {this.state.activeTabKey === 1 && this.renderRecordByProcess(PerformanceProcess.Self)}
                    </Tab>
                    <Tab eventKey={2} title={getResource('ProjectManager')}>
                        {this.state.activeTabKey === 2 && this.renderRecordByProcess(PerformanceProcess.ProjectManager)}
                    </Tab>
                    <Tab eventKey={3} title={getResource('Evaluator')}>
                        {this.state.activeTabKey === 3 && this.renderRecordByProcess(PerformanceProcess.Evaluator)}
                    </Tab>
                    <Tab eventKey={4} title={getResource('Reviewer')}>
                        {this.state.activeTabKey === 4 && this.renderRecordByProcess(PerformanceProcess.Reviewer)}
                    </Tab>
                </Tabs>
            </div>
        );
    }
}