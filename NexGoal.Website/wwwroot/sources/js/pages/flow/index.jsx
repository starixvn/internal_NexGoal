import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import Filter from './Filter';
import Table from './Table';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isHost: document.getElementById('IsHost').checked,
            filter: {
                targetTypeId: document.getElementById('TargetTypeId').value,
                isActive: document.getElementById('IsActive').value.toLowerCase()
            },
            targetTypes: [],
            flowTemplates: []
        };

        this.handleOnFilterChange = this.handleOnFilterChange.bind(this);
        this.handleOnSubmit = this.handleOnSubmit.bind(this);
    }

    componentDidMount() {
        this.getTargetTypes();
        this.getFlowTemplates();
    }

    getTargetTypes() {
        axios.get(resolveClientUrl('Common/GetTargetTypes'))
            .then(response => {
                this.setState({ targetTypes: response.data });
            }).catch(axiosError);
    }

    getFlowTemplates() {
        const filter = this.state.filter;
        axios.get(resolveClientUrl('Flow/GetAll'), {
            params: {
                targetTypeId: filter.targetTypeId,
                isActive: filter.isActive
            }
        }).then(response => {
            const data = response.data;
            data.map((e, index) => e.rowNum = index + 1);

            this.setState({ flowTemplates: data });
        }).catch(axiosError);
    }

    handleOnFilterChange(filter) {
        this.setState({ filter: filter });
    }

    handleOnSubmit() {
        this.getFlowTemplates();
    }

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">{getResource('EvaluationFlowManagement')}</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <Filter filter={this.state.filter}
                            targetTypes={this.state.targetTypes}
                            onFilterChange={this.handleOnFilterChange}
                            onSubmit={this.handleOnSubmit}
                        />

                        <Table isHost={this.state.isHost}
                            targetTypes={this.state.targetTypes}
                            flowTemplates={this.state.flowTemplates}
                            onFilterSubmit={this.handleOnSubmit}
                        />
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('evaluationflow-wrapper'));