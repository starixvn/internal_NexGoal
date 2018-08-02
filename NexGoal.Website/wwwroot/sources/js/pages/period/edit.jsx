import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';
import Breadcrumb from '../../shared/components/Breadcrumb';
import ToastLayout from '../../shared/layouts/ToastLayout';
import EditBasicInformation from './EditBasicInformation';

export default class EditPeriod extends Component {
    constructor(props) {
        super(props);

        this.handleSelect = this.handleSelect.bind(this);

        this.state = {
            periodId: document.getElementById('periodId').value,
            activeKey: 1,
            loadProcessTab: false,
            loadEmployeeTab: false,
        };
    }

    handleSelect(key) {
        if (key === 1) {
            this.setState({ activeKey: key });
        }

        if (key === 2 && !this.state.loadProcessTab) {
            this.setState({ loadProcessTab: true });
        }

        if (key === 3 && !this.state.loadEmployeeTab) {
            this.setState({ loadEmployeeTab: true });
        }

        switch (key) {
            case 1:
                this.setState({ activeKey: key });
                break;
            case 2:
                this.setState({ activeKey: key, loadProcessTab: true });
                break;
            case 3:
                this.setState({ activeKey: key, loadEmployeeTab: true });
                break;
        }
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">Edit Evaluation Period</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <Tabs id="tabs"
                        activeKey={this.state.activeKey}
                        onSelect={this.handleSelect}>

                        <Tab eventKey={1} title="Basic Information">
                            <EditBasicInformation periodId={this.state.periodId} />
                        </Tab>
                        <Tab eventKey={2} title="Process Setting">

                        </Tab>
                        <Tab eventKey={3} title="Employee Setting">
                        </Tab>
                    </Tabs>
                </div>
                <ToastLayout />

            </div>

        );
    }
}

ReactDOM.render(<EditPeriod />, document.getElementById('edit-period-wrapper'));