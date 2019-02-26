import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { Nav, NavItem, Tab } from 'react-bootstrap';

import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import getResource from '../../resourceHelper';

import InformationModal from './focus/InformationModal';
import PAFocus from './focus/index';
import Chart from './chart/index';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowModal: false,
            activeKey: 1,
            isFirstLoad: false
        };
    }

    handleSelect = (key) => {
        this.setState({ activeKey: key });

        if (key === 2 && !this.state.isFirstLoad) {
            this.setState({ isFirstLoad: true });
        }
    }

    handleShowModal = () => {
        this.setState({
            isShowModal: true
        });
    }

    handleCloseModal = () => {
        this.setState({ isShowModal: false });
    }

    render() {
        return (
            <ToastLayout>
                <div className="row content-wrapper">
                    <Breadcrumb>
                        <li className="current">{`${getResource('ContributionAndRanking')} (${getResource('PAFocus')})`}</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <Tab.Container id="tabs-pafocus"
                            activeKey={this.state.activeKey}
                            onSelect={this.handleSelect}
                        >
                            <Fragment>
                                <Nav bsStyle="tabs">
                                    <NavItem eventKey={1}>
                                        {getResource('PAFocus')}
                                        <i className="fa fa-info-circle" onClick={this.handleShowModal} />
                                    </NavItem>
                                    <NavItem eventKey={2}>{getResource('Chart')}</NavItem>
                                </Nav>
                                <Tab.Content animation>
                                    <Tab.Pane eventKey={1}>
                                        <PAFocus />
                                    </Tab.Pane>
                                    <Tab.Pane eventKey={2}>
                                        {
                                            this.state.isFirstLoad &&
                                            <Chart />
                                        }
                                    </Tab.Pane>
                                </Tab.Content>
                            </Fragment>
                        </Tab.Container>
                    </div>
                </div>
                {
                    this.state.isShowModal &&
                    <InformationModal
                        isShow={this.state.isShowModal}
                        onClose={this.handleCloseModal}
                    />
                }
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('pafocus-wrapper'));