import React, { Component } from 'react';
import { Button, Form, FormGroup, Modal } from 'react-bootstrap';

import commonFunctions from '../../../common-functions';

export default class CreateEOQ extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoq: {
                quarter: 1,
                year: new Date().getFullYear(),
                description: ''
            }
        };

        this.handleQuarterChange = this.handleQuarterChange.bind(this);
        this.handleYearChange = this.handleYearChange.bind(this);
    }

    handleQuarterChange(event) {
        this.setState({ eoq: Object.assign({}, this.state.eoq, { quarter: event.target.value }) });
    }

    handleYearChange(event) {
        this.setState({ eoq: Object.assign({}, this.state.eoq, { year: event.target.value }) });
    }

    handleSubmit() {
    }

    render() {
        const currentYear = new Date().getFullYear();
        return (
            <Modal show={this.props.showModal} onHide={this.props.closeModal} dialogClassName="custom-modal modal-md">
                <Modal.Header closeButton>
                    <Modal.Title>Create Nomination Period</Modal.Title>
                </Modal.Header>
                <Form id="create-eoq-form" horizontal onSubmit={this.handleSubmit}>
                    <Modal.Body>
                        <FormGroup>
                            <label className="col-sm-3 control-label required-label">Quarter</label>
                            <div className="col-sm-3 field">
                                <select className="form-control" onChange={this.handleQuarterChange}>
                                    <option value="1">1st</option>
                                    <option value="2">2nd</option>
                                    <option value="3">3rd</option>
                                    <option value="4">4th</option>
                                </select>
                            </div>
                            <label className="col-sm-3 control-label required-label">Year</label>
                            <div className="col-sm-3 field">
                                <select className="form-control" onChange={this.handleYearChange}>
                                    <option value={currentYear}>{currentYear}</option>
                                    <option value={currentYear + 1}>{currentYear + 1}</option>
                                </select>
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <label className="col-sm-3 control-label required-label">Start Date For Nominator</label>
                            <div className="col-sm-3 field">
                            </div>
                            <label className="col-sm-3 control-label required-label">End Date For Nominator</label>
                            <div className="col-sm-3 field">
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <label className="col-sm-3 control-label required-label">Remind Date For Nominator</label>
                            <div className="col-sm-3 field">
                            </div>
                            <label className="col-sm-3 control-label required-label">End Date For Committee</label>
                            <div className="col-sm-3 field">
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <label className="col-sm-3 control-label">Description</label>
                            <div className="col-sm-9 field">
                                <textarea className="form-control comment none-resize" rows="5"></textarea>
                            </div>
                        </FormGroup>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button onClick={this.props.closeModal} className="btn-cancel">Cancel</Button>
                        <Button type="submit" className="btn btn-primary">Submit</Button>
                    </Modal.Footer>
                </Form>
            </Modal>
        );
    }
}