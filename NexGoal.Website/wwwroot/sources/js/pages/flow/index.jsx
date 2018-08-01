import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import commonFunctions from '../../common-functions';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import Filter from './Filter';
import Table from './Table';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isHost: false,
            filter: {
                targetTypeId: '',
                isActive: 'true'
            },
            targetTypes: [],
            flowTemplates: []
        };

        this.handleFilterChange = this.handleFilterChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    componentWillMount() {
        this.setState({
            isHost: document.getElementById('IsHost').checked,
            filter: {
                targetTypeId: document.getElementById('TargetTypeId').value,
                isActive: document.getElementById('IsActive').value.toLowerCase()
            }
        });

        this.getTargetTypes();
        this.getFlowTemplates();
    }

    getTargetTypes() {
        axios.get(commonFunctions.getRootUrl() + 'Common/GetTargetTypes')
            .then(response => {
                this.setState({ targetTypes: response.data });
            }).catch(commonFunctions.axiosError);
    }

    getFlowTemplates() {
        axios.get(commonFunctions.getRootUrl()
            + `Flow/GetFlowTemplates?targetTypeId=${this.state.filter.targetTypeId}`
            + `&isActive=${this.state.filter.isActive}`)
            .then(response => {
                const data = response.data;
                data.map((e, index) => e.rowNum = index + 1);

                this.setState({ flowTemplates: data });
            }).catch(commonFunctions.axiosError);
    }

    handleFilterChange(filter) {
        this.setState({ filter: filter });
    }

    handleSubmit() {
        this.getFlowTemplates();
    }

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">Evaluation Flow Management</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <Filter filter={this.state.filter}
                            targetTypes={this.state.targetTypes}
                            handleFilterChange={this.handleFilterChange}
                            handleFilterSubmit={this.handleSubmit} />

                        <Table isHost={this.state.isHost}
                            targetTypes={this.state.targetTypes}
                            flowTemplates={this.state.flowTemplates}
                            handleFilterSubmit={this.handleSubmit} />
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('flowtemplate-wrapper'));